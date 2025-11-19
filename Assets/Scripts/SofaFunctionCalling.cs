using LLMUnity;
using SofaUnity;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Args {
    // translation
    public float translateX;
    public float translateY;
    public float translateZ;

    // rotate liver
    public float degrees;
    public string axis;

    // set gravity
    public float gravX;
    public float gravY;
    public float gravZ;

    // load patient
    public string name;

    // highlight segment
    public int segment;
}


[System.Serializable]
public class LLMResult {
    public string choice;
    public string response;

    public Args args;
}


public class SofaFunctionCalling : MonoBehaviour {

    [SerializeField] private LLMCharacter llmCharacter;
    [SerializeField] private TMP_InputField userText;
    [SerializeField] private TMP_Text aiText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private SofaContext sofaContext;

    void Start() {
        // find the DAG node named "Liver"
        foreach (var node in sofaContext.GetComponentsInChildren<SofaDAGNode>()) {
            if (node.DisplayName == "Liver") {
                LLMFunctions.liverNode = node;
                break;
            }
        }
        if (LLMFunctions.liverNode == null)
            Debug.LogError("Liver node not found!");
        LLMFunctions.sofaContext = sofaContext;

        userText.onSubmit.AddListener(OnInputFieldSubmit);

        Debug.Log("Created with CreateJSONSchema:\n" + CreateJSONSchema());

        // use JSON schema grammar for LLM ouput
        llmCharacter.grammarJSONString = CreateJSONSchema();
    }

    private async void OnInputFieldSubmit(string message) {
        if (!string.IsNullOrEmpty(message)) {

            //disable user input
            userText.interactable = false;

            //wait for response from llmCharacter
            string jsonResponse = await llmCharacter.Chat(ConstructPrompt(message));

            Debug.Log(jsonResponse);

            // convert response form JSON to variables (LLMResult Class)
            LLMResult result = null;
            try { result = JsonUtility.FromJson<LLMResult>(jsonResponse); }
            catch {
                aiText.text = $"ERROR in converting from JSON Schema to C# variables : {jsonResponse}";
                userText.interactable = true;
                return;
            }


            // Call function based on result or None
            if (result.choice != "None") {
                string functionText = CallFunction(result);
                aiText.text = $"Function Choice: {result.choice}\nFunction Response: {functionText}";
            }
            else {
                aiText.text = $"Function Choice: None";
            }

            aiText.text += "\n\n" + result.response;


            //enable user input
            userText.interactable = true;
            userText.Select();
        }
    }

    // call function based on LLM result
    private string CallFunction(LLMResult result) {
        switch (result.choice) {
            case "TranslateLiver":
                return LLMFunctions.TranslateLiver(result.args.translateX, result.args.translateY, result.args.translateZ);
            case "RotateLiver":
                return LLMFunctions.RotateLiver(result.args.degrees, result.args.axis);
            case "SetGravity":
                return LLMFunctions.SetGravity(result.args.gravX, result.args.gravY, result.args.gravZ);
            case "LoadPatientInfo":
                return LLMFunctions.LoadPatientInfo(result.args.name);
            case "HighlightSegment":
                return LLMFunctions.HighlightSegment(result.args.segment);
            // for no arg functions
            default:
                var f = getLLMFunction(result.choice);
                return (string)f.Invoke(null, null);
        }
    }

    // construct prompt to give to LLM based on list of functions as well as user input
    private string ConstructPrompt(string message) {
        string choices = string.Join(", ", GetFunctionNames());
        return "You are an AI assistant. You are part of a simulation that contains a soft body simulation of a liver model." +
                "The simulation/liver model can be controlled with several functions." +
                "You also have access to patient information." +
                "Pick the best function out of these choices based on the user input. " +
                "PICK 'None' IF NO OPTION FITS OR IF YOU DON'T HAVE ENOUGH INFORMATION FOR THE ARGUMENTS OF THE FUNCTION. \n" +
                "In addition to that, write a natural language reply for the user.\n" +
               $"User input: {message}\n\n" +
               $"Function Choices: {choices}\n\n" +
               $"Funciton Descriptions: {LLMFunctions.functionDescriptions}";
    }

    private string CreateJSONSchema() {
        // dictionary containing schemas for functions with arguments
        Dictionary<string, string> functionsWithArgumentsSchemas = new Dictionary<string, string> {
            {
                "TranslateLiver",
                @"
                {
                ""type"": ""object"",
                ""properties"": {
                    ""translateX"": { ""type"": ""number""},
                    ""translateY"": { ""type"": ""number""},
                    ""translateZ"": { ""type"": ""number""}
                },
                ""required"": [""translateX"", ""translateY"", ""translateZ""],
                ""additionalProperties"" : false
                }"
            },
            {
                "RotateLiver",
                @"
                {
                ""type"": ""object"",
                ""properties"": {
                    ""degrees"": { ""type"": ""number"" },
                    ""axis"": { ""type"": ""string"", ""enum"" : [""x"", ""y"", ""z""] }
                },
                ""required"": [""degrees"", ""axis""],
                ""additionalProperties"" : false
                }"
            },
            {
                "SetGravity",
                @"
                {
                ""type"": ""object"",
                ""properties"": {
                    ""gravX"": { ""type"": ""number""},
                    ""gravY"": { ""type"": ""number""},
                    ""gravZ"": { ""type"": ""number""}
                },
                ""required"": [""gravX"", ""gravY"", ""gravZ""],
                ""additionalProperties"" : false
                }"
            },
            {
                "LoadPatientInfo",
                @"
                {
                ""type"": ""object"",
                ""properties"": {
                    ""name"": { ""type"": ""string""}
                },
                ""required"": [""name""],
                ""additionalProperties"" : false
                }"
            },
            {
                "HighlightSegment",
                @"
                {
                ""type"": ""object"",
                ""properties"": {
                    ""segment"": { ""type"": ""integer"", ""minimum"": 1, ""maximum"": 9}
                },
                ""required"": [""segment""],
                ""additionalProperties"" : false
                }"
            }
        };

        var functions = getLLMFunctions();
        var subSchemas = new List<string>();

        // add None as option
        subSchemas.Add(CreateNoArgumentsSchema("None"));

        // create subSchemas and add them to list for all functions with arguments
        foreach (var f in functionsWithArgumentsSchemas) {
            subSchemas.Add(CreateArgumentsSchema(f.Key, f.Value));
        }

        // creat subSchemas and add to list for functions with no arguments
        foreach (var f in functions) {
            if (f.GetParameters().Length == 0 && !functionsWithArgumentsSchemas.ContainsKey(f.Name)) {
                subSchemas.Add(CreateNoArgumentsSchema(f.Name));
            }
        }


        string schema =
            @$" 
            {{""oneOf"": [ 
                {string.Join(",\n", subSchemas)}
            ]}}";

        return schema;
    }

    // creates and returns branch for oneOf in JSON schema for a function with arguments
    private string CreateArgumentsSchema(string name, string argsSchema) {
        return
            $@"
            {{
            ""type"": ""object"",
            ""properties"": {{
                ""choice"": {{ ""type"": ""string"", ""enum"": [""{name}""]}},
                ""response"": {{ ""type"": ""string"" }},
                ""args"":{argsSchema}
            }},
            ""required"": [""choice"", ""response"", ""args""],
            ""additionalProperties"": false
            }}";
    }

    // creates and return branch for oneOf for func with no arguments
    private string CreateNoArgumentsSchema(string name) {
        return
        $@"
        {{
          ""type"": ""object"",
          ""properties"": {{
            ""choice"": {{ ""type"": ""string"", ""enum"": [""{name}""] }},
            ""response"": {{ ""type"": ""string"" }}
          }},
          ""required"": [""choice"", ""response""],
          ""additionalProperties"": false
        }}";
    }

    private MethodInfo[] getLLMFunctions() {
        return typeof(LLMFunctions).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
    }

    private MethodInfo getLLMFunction(string functionName) {
        return typeof(LLMFunctions).GetMethod(functionName, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
    }

    private string[] GetFunctionNames() {
        List<string> functionNames = new List<string>();
        foreach (var function in getLLMFunctions())
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }
}

