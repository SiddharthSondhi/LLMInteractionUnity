using LLMUnity;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CubeArgs {
    // rotate cube
    public float degrees;
    public string axis;

    // scale cube
    public float scaleFactor;

    // set cube color
    public int r;
    public int g;
    public int b;
}


[System.Serializable]
public class CubeLLMResult {
    public string choice;
    public string response;

    public CubeArgs args;
}


public class CubeFunctionCalling : MonoBehaviour {

    [SerializeField] private LLMCharacter llmCharacter;
    [SerializeField] private TMP_InputField userText;
    [SerializeField] private TMP_Text aiText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform cube;

    void Start() {
        CubeFunctions.cube = cube;

        userText.onSubmit.AddListener(OnInputFieldSubmit);

        Debug.Log("Old JSON Schema:\n" + CreateJSONSchemaOld());
        Debug.Log("Test JSON Schema:\n" + TestJasonSchema);
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
            CubeLLMResult result = null;
            try { result = JsonUtility.FromJson<CubeLLMResult>(jsonResponse); }
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
    private string CallFunction(CubeLLMResult result) {
        switch (result.choice) {
            case "RotateCube":
                return CubeFunctions.RotateCube(result.args.degrees, result.args.axis);
            case "ScaleCube":
                return CubeFunctions.ScaleCube(result.args.scaleFactor);
            case "SetCubeColor":
                return CubeFunctions.SetCubeColor(result.args.r, result.args.g, result.args.b);

            default:
                var f = getLLMFunction(result.choice);
                return (string)f.Invoke(null, null);
        }
    }

    // construct prompt to give to LLM based on list of functions as well as user input
    private string ConstructPrompt(string message) {
        string choices = string.Join(", ", GetFunctionNames());
        return "Pick the best function out of these choices based on the user input. " +
                "PICK 'None' IF NO OPTION FITS OR IF YOU DON'T HAVE ENOUGH INFORMATION FOR THE ARGUMENTS OF THE FUNCTION. \n" +
                "In addition to that, write a natural language reply for the user.\n" +
               $"User input: {message}\n\n" +
               $"Function Choices: {choices}\n\n" +
               $"Funciton Descriptions: {CubeFunctions.functionDescriptions}";
    }

    private string CreateJSONSchema() {
        // dictionary containing schemas for functions with arguments
        Dictionary<string, string> functionsWithArgumentsSchemas = new Dictionary<string, string> {
            {
                "RotateCube",
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
                "ScaleCube",
                @"
                {
                ""type"": ""object"",
                ""properties"": {
                    ""scaleFactor"": { ""type"": ""number"", ""minimum"": 0.0 }
                },
                ""required"": [""scaleFactor""],
                ""additionalProperties"" : false
                }"
            },
            {
                "SetCubeColor",
                @"
                {
                ""type"": ""object"",
                ""properties"": {
                    ""r"": { ""type"": ""number"", ""minimum"": 0, ""maximum"": 255 },
                    ""g"": { ""type"": ""number"", ""minimum"": 0, ""maximum"": 255 },
                    ""b"": { ""type"": ""number"", ""minimum"": 0, ""maximum"": 255 }
                },
                ""required"": [""r"", ""g"", ""b""],
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
        return typeof(CubeFunctions).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
    }

    private MethodInfo getLLMFunction(string functionName) {
        return typeof(CubeFunctions).GetMethod(functionName, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
    }

    private string[] GetFunctionNames() {
        List<string> functionNames = new List<string>();
        foreach (var function in getLLMFunctions())
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }


    private string CreateJSONSchemaOld() {
        string functionNames = "\"" + string.Join("\",\"", GetFunctionNames()) + "\"";

        // Also have None as an option
        functionNames += ",\"None\"";

        return
        $@"
        {{
          ""type"": ""object"",
          ""properties"": {{
            ""choice"": {{ ""type"": ""string"", ""enum"": [ {functionNames} ] }},
            ""response"": {{ ""type"": ""string"" }}
          }},
          ""required"": [""choice"", ""response""],
          ""additionalProperties"": false
        }}";
    }

    private string TestJasonSchema =
       @"
{
  ""oneOf"": [
    {
      ""type"": ""object"",
      ""properties"": {
        ""choice"": { ""type"": ""string"", ""enum"": [""RotateCube""] },
        ""response"": { ""type"": ""string"" },
        ""args"": {
          ""type"": ""object"",
          ""properties"": {
            ""degrees"": { ""type"": ""number"" },
            ""axis"": { ""type"": ""string"", ""enum"": [""x"", ""y"", ""z""] }
          },
          ""required"": [""degrees"", ""axis""],
          ""additionalProperties"": false
        }
      },
      ""required"": [""choice"", ""response"", ""args""],
      ""additionalProperties"": false
    },
    {
      ""type"": ""object"",
      ""properties"": {
        ""choice"": { ""type"": ""string"", ""enum"": [""MoveCubeUp""] },
        ""response"": { ""type"": ""string"" }
      },
      ""required"": [""choice"", ""response""],
      ""additionalProperties"": false
    },
    {
      ""type"": ""object"",
      ""properties"": {
        ""choice"": { ""type"": ""string"", ""enum"": [""MoveCubeDown""] },
        ""response"": { ""type"": ""string"" }
      },
      ""required"": [""choice"", ""response""],
      ""additionalProperties"": false
    },
    {
      ""type"": ""object"",
      ""properties"": {
        ""choice"": { ""type"": ""string"", ""enum"": [""None""] },
        ""response"": { ""type"": ""string"" }
      },
      ""required"": [""choice"", ""response""],
      ""additionalProperties"": false
    }
  ]
}";
}

/* TODO
- fix: LLM tends to call functions instead of picking None 
  eg: when asked to make object bigger, scales by factor of 0 by default and ask for more info
- automatic argument schema construction
- allow multiple function calling for same prompt
- better way of converting from JSON to arguments (use Newtonsoft.Json?)
- better way of handeling function descriptions (some way to attach to each function?)
- voice recognition
- RAG (test)
 */
