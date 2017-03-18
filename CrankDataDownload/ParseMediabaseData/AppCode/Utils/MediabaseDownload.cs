using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseMediabaseData.AppCode.Utils
{
    public class MediabaseDownload
    {
        string mediabaseFilePath;


        //To hold ouptut text
        StringBuilder outputText = new StringBuilder();
        string errorText = "";
        public MediabaseDownload(string filePath = "")
        {
            mediabaseFilePath = filePath;
        }

        public int StartMediabaseDownload()
        {
            int exitCode = 0;
            if (String.IsNullOrEmpty(mediabaseFilePath ))
            {
                throw new ArgumentNullException("Invalid Mediabase download script path");
            }

            Process mediabaseDownloadProcess = new Process();

            ProcessStartInfo mediabaseProcessStartInfo = new ProcessStartInfo();

            //Required to luanch the process without full path
            mediabaseProcessStartInfo.UseShellExecute = false;

            mediabaseProcessStartInfo.RedirectStandardError = true;
            mediabaseProcessStartInfo.RedirectStandardOutput = true;
            mediabaseProcessStartInfo.FileName = mediabaseFilePath;

            //Add event handler 
            mediabaseDownloadProcess.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
            mediabaseDownloadProcess.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);

            mediabaseDownloadProcess.StartInfo = mediabaseProcessStartInfo;
            mediabaseDownloadProcess.Start();


            mediabaseDownloadProcess.BeginErrorReadLine();
            mediabaseDownloadProcess.BeginOutputReadLine();

            mediabaseDownloadProcess.WaitForExit();

            exitCode = mediabaseDownloadProcess.ExitCode;
            mediabaseDownloadProcess.Close();

            return exitCode;
        }

        private void ErrorDataHandler(object sender, DataReceivedEventArgs errorLine)
        {
            if (!String.IsNullOrEmpty(errorLine.Data))
            {
                errorText += errorLine.Data;
            }
        }

        private void OutputDataHandler(object sender, DataReceivedEventArgs outLine)
        {
            if(!String.IsNullOrEmpty(outLine.Data))
            {
                outputText.AppendLine(outLine.Data);
                Console.WriteLine(outLine.Data);
            }
        }
    }
}

