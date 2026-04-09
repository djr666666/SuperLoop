
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ProtoBufferTools : Editor
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("MyTools/Config/ProtoToCSharp")]
    public static void GenerateCSharoForProtoBuffer()
    {

        TransferProtpToCSharp();
    }

    private static void TransferProtpToCSharp()
    {

        string exePath = Application.dataPath + "/../GoogleProtobuf/protoc.exe";
        string srcPath = Application.dataPath + "/../GoogleProtobuf/apis/";
        string desPath = Application.dataPath + "/GoogleProtobuf/Generate";
        string strFiles = string.Empty;

        //创建输出目录
        Directory.CreateDirectory(desPath);
        //从输入目录获取所有文件的路径名称，并通过空格链接
        var scrPathInfo = Directory.CreateDirectory(srcPath);
        var flies = scrPathInfo.GetFiles();
        foreach (var item in flies)
        {
            //检查后缀
            if (item.Name.EndsWith(".proto"))
            {
                strFiles += " " + item.Name;
            }      
        }
        //拼接参数
        string argument = $"--proto_path={srcPath} --csharp_out={desPath}{strFiles}";
        //执行转换操作
        GenerateCSharp(exePath, argument);
        //刷新
        AssetDatabase.Refresh();
    }

    static void GenerateCSharp(string exePath, string argument)
    {
        //工程项目不能有空格 文件夹避免出现空格
        try{

            Process process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = argument;
            process.EnableRaisingEvents = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            var processRes = new StringBuilder();
            //搜集exe运行输出信息
            process.OutputDataReceived += new DataReceivedEventHandler
                (

                delegate(object sender,DataReceivedEventArgs e)
                { 
                    processRes.Append(e.Data);
                    processRes.Append("     ");
                }
                );
            //搜集exe运行报错信息
            process.ErrorDataReceived += new DataReceivedEventHandler(

                 delegate (object sender, DataReceivedEventArgs e)
                 {
                     processRes.Append(e.Data);
                     processRes.Append("     ");
                 }

                );

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                UnityEngine.Debug.Log($"protos To CSharp Finished : exit Code  = {process.ExitCode} \n error:{processRes}");
            }
            else
            {
                UnityEngine.Debug.Log($"protos To CSharp successull!!!");
            }


        }
        catch(Exception e) {
            UnityEngine.Debug.Log($"protos To CSharp faidle{e}");
        }
    }
}
