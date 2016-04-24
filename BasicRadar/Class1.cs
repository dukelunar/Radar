using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using ArcheBuddy.Bot.Classes;
using BasicRadar;

namespace DefaultNameSpace
{
    public class DefaultClass : Core
    {
        public static string GetPluginAuthor()
        {
            return "Liyalai";
        }

        public static string GetPluginVersion()
        {
            return "1.0.0.0";
        }

        public static string GetPluginDescription()
        {
            return "BasicRadar";
        }

        private Thread formThread;
        private Form1 theForm;
        public bool formIsOpen;


        //Call on plugin start
        public void PluginRun()
        {

            formThread = new Thread(RunForm);
            formThread.SetApartmentState(ApartmentState.STA);
            formThread.Start();
            formIsOpen = true;
            while (formIsOpen)
            {
                Thread.Sleep(1000);
            }
            
            PluginStop();

        }

        private void RunForm()
        {
            theForm = new Form1(this, me);
            System.Windows.Forms.Application.Run(theForm);
        }

        //Call on plugin stop
        public void PluginStop()
        {
            theForm.Close();
        }
    }
}