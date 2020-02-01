/* ========================================================================
 * Copyright (C) 2020 Joe Clapis.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ======================================================================== */

using GPhoto2.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestConsole
{
    class Program
    {
        private static Context Context;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                Context = new Context();
                Context.IdleNotification += Context_IdleNotification;
                Context.ProgressStarted += Context_ProgressStarted;
                Context.ProgressUpdated += Context_ProgressUpdated;
                Context.ProgressStopped += Context_ProgressStopped;
                Context.ErrorOccurred += Context_ErrorOccurred;
                Context.StatusNotification += Context_StatusNotification;
                Context.QuestionAsked += Context_QuestionAsked;
                //Context.CancelRequested += Context_CancelRequested;
                Context.MessageReceived += Context_MessageReceived;

                Console.WriteLine("Context loaded. Press Enter to scan for cameras.");
                Console.ReadLine();
                IReadOnlyList<Camera> cameras = Context.GetCameras();
                Console.WriteLine($"Detected {cameras.Count()} camera(s).");
                Console.WriteLine();

                for(int i = 0; i < cameras.Count(); i++)
                {
                    Camera camera = cameras[i];
                    Console.WriteLine($"Camera {i + 1}:");
                    Console.WriteLine($"\tModel Name: {camera.ModelName}");
                    Console.WriteLine($"\tDriver Quality: {camera.DriverQuality}");
                    Console.WriteLine($"\tConnection Type: {camera.ConnectionType}");

                    Console.WriteLine($"\tSupported serial speeds ({camera.SupportedSerialSpeeds.Count}):");
                    foreach(int speed in camera.SupportedSerialSpeeds)
                    {
                        Console.WriteLine($"\t\t{speed}");
                    }

                    Console.WriteLine($"\tSupported capture functions:");
                    CameraOperation supportedCaptureFunctions = camera.SupportedCaptureFunctions;
                    foreach(CameraOperation operation in Enum.GetValues(typeof(CameraOperation)))
                    {
                        if(supportedCaptureFunctions.HasFlag(operation))
                        {
                            Console.WriteLine($"\t\t{operation}");
                        }
                    }

                    Console.WriteLine($"\tSupported file functions:");
                    CameraFileOperation supportedFileFunctions = camera.SupportedFileFunctions;
                    foreach (CameraFileOperation operation in Enum.GetValues(typeof(CameraFileOperation)))
                    {
                        if (supportedFileFunctions.HasFlag(operation))
                        {
                            Console.WriteLine($"\t\t{operation}");
                        }
                    }

                    Console.WriteLine($"\tSupported folder functions:");
                    CameraFolderOperation supportedFolderFunctions = camera.SupportedFolderFunctions;
                    foreach (CameraFolderOperation operation in Enum.GetValues(typeof(CameraFolderOperation)))
                    {
                        if (supportedFolderFunctions.HasFlag(operation))
                        {
                            Console.WriteLine($"\t\t{operation}");
                        }
                    }

                    Console.WriteLine("\tUSB Info:");
                    Console.WriteLine($"\t\tVendor ID: 0x{camera.USBInfo.VendorID.ToString("X8")}");
                    Console.WriteLine($"\t\tProduct ID: 0x{camera.USBInfo.ProductID.ToString("X8")}");
                    Console.WriteLine($"\t\tClass: 0x{camera.USBInfo.Class.ToString("X8")}");
                    Console.WriteLine($"\t\tSubclass: 0x{camera.USBInfo.Subclass.ToString("X8")}");
                    Console.WriteLine($"\t\tProtocol: 0x{camera.USBInfo.Protocol.ToString("X8")}");
                    Console.WriteLine();
                }

                Console.WriteLine("Done.");
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.GetType().Name} - {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            finally
            {
                Context.Dispose();
            }
        }

        private static void Context_MessageReceived(object sender, string e)
        {
            Console.WriteLine($"New message from the Context: {e}");
        }

        private static void Context_CancelRequested(object sender, EventArgs e)
        {
            Console.WriteLine($"Context asks if you want to cancel.");
            Console.Write("Response (y/n): ");

            string line = Console.ReadLine();
            while (true)
            {
                if (line == "y")
                {
                    Context.SetCancelResponse(GPContextFeedback.Cancel);
                    break;
                }
                else if (line == "n")
                {
                    Context.SetCancelResponse(GPContextFeedback.OK);
                    break;
                }
                else
                {
                    Console.Write("Invalid Response. Response (y/n): ");
                    line = Console.ReadLine();
                }
            }
        }

        private static void Context_QuestionAsked(object sender, string e)
        {
            Console.WriteLine($"Context asks: {e}");
            Console.Write("Response (y/n): ");

            string line = Console.ReadLine();
            while(true)
            {
                if (line == "y")
                {
                    Context.SetQuestionResponse(GPContextFeedback.OK);
                    break;
                }
                else if(line == "n")
                {
                    Context.SetQuestionResponse(GPContextFeedback.Cancel);
                    break;
                }
                else
                {
                    Console.Write("Invalid Response. Response (y/n): ");
                    line = Console.ReadLine();
                }
            }
        }

        private static void Context_StatusNotification(object sender, string e)
        {
            Console.WriteLine($"Context status updated: {e}");
        }

        private static void Context_ErrorOccurred(object sender, string e)
        {
            Console.WriteLine($"Context encountered an error: {e}");
        }

        private static void Context_ProgressStopped(object sender, uint e)
        {
            Console.WriteLine($"Event {e} completed.");
        }

        private static void Context_ProgressUpdated(object sender, ProgressUpdateArgs e)
        {
            Console.WriteLine($"Progress on event {e.ProgressID}: {e.Progress.ToString("P2")}");
        }

        private static void Context_ProgressStarted(object sender, ProgressStartArgs e)
        {
            Console.WriteLine($"Progress on event {e.ProgressID} stared: {e.Message}");
        }

        private static void Context_IdleNotification(object sender, EventArgs e)
        {
            Console.WriteLine("Context is now idle.");
        }

    }
}
