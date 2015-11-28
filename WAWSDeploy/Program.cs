﻿using System;
using System.Diagnostics;
using Microsoft.Web.Deployment;

namespace WAWSDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                WriteLine(@"WAWSDeploy version {0}", typeof(Program).Assembly.GetName().Version);
                WriteLine(@"Usage: WAWSDeploy.exe c:\SomeFolder MySite.PublishSettings [flags]");
                WriteLine(@"Options:");
                WriteLine(@" /p  /password: provide the password if it's not in the profile");
                WriteLine(@" /d  /DeleteExistingFiles: delete target files that don't exist at the source");
                WriteLine(@" /au /AllowUntrusted: skip cert verification");
                WriteLine(@" /v  /Verbose: Verbose mode");
                WriteLine(@" /w  /WhatIf: don't actually perform the publishing");
                WriteLine(@" /usechecksum: use checksum instead of last write timestamp to compare files");
                WriteLine(@" /t  /TargetPath: the virtual or physical directory to deploy to");
                return;
            }

            // parse the command line args
            var command = Args.Configuration.Configure<DeploymentArgs>().CreateAndBind(args);

            try
            {
                var webDeployHelper = new WebDeployHelper();

                webDeployHelper.DeploymentTraceEventHandler += Trace;


                WriteLine("Starting deployment...");
                DeploymentChangeSummary changeSummary = webDeployHelper.DeployContentToOneSite(
                    command.Folder, 
                    command.PublishSettingsFile, 
                    command.Password, 
                    command.AllowUntrusted,
                    !command.DeleteExistingFiles,
                    command.TraceLevel,
                    command.WhatIf,
                    command.UseChecksum,
                    command.TargetPath
                    );

                WriteLine("BytesCopied: {0}", changeSummary.BytesCopied);
                WriteLine("Added: {0}", changeSummary.ObjectsAdded);
                WriteLine("Updated: {0}", changeSummary.ObjectsUpdated);
                WriteLine("Deleted: {0}", changeSummary.ObjectsDeleted);
                WriteLine("Errors: {0}", changeSummary.Errors);
                WriteLine("Warnings: {0}", changeSummary.Warnings);
                WriteLine("Total changes: {0}", changeSummary.TotalChanges);
            }
            catch (Exception e)
            {
                WriteLine("Deployment failed: {0}", e.Message);
                Environment.ExitCode = 1;
            }
        }

        static void Trace(object sender, DeploymentTraceEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        static void WriteLine(string message, params object[] args)
        {
            Console.WriteLine(String.Format(message, args));
        }
    }
}
