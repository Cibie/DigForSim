using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [HideInInspector]
    public Dropdown dropdown;
    [HideInInspector]
    public string filename;

    public static SceneController inst;

    void Awake()
    {
        if (inst != null) // Creating an instance of the SceneController 
                          // that can be called from other objects to retrieve
                          // the configuration file path
            return;
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PopulateList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PopulateList() // Populate the dropdown menu of the menu scene with
                               // all files in the "ConfigFiles" directory
    {
        dropdown.ClearOptions(); //Clear the dropdown menu from default example options 
        List<string> filesName = new List<string>() { };
        DirectoryInfo dir = new DirectoryInfo("./ConfigFiles/");
        FileInfo[] files = dir.GetFiles("*.*"); // create an array with all files of the directory
        foreach (FileInfo f in files) // for each file in the array add an element in the List
        {
            filesName.Add(f.Name);
        }
        dropdown.AddOptions(filesName); // Add all the elements of the list as option of the dropdown menu

    }

    public void GetConfigFile()
    {
        DontDestroyOnLoad(transform.gameObject); // The SceneController will be kept also when loading the other scene
        filename = dropdown.options[dropdown.value].text; // Getting the selected item of the dropdown menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"); //loading the other scene
    }
}
