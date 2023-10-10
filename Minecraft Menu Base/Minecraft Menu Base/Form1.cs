#region "References"
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PS3Lib;
#endregion

#region "Namespace"
namespace Minecraft_Menu_Base
{

    #region "Partial Class"
    public partial class Form1 : Form
    {

        #region "Variables"
        public static int y, x;
        public static string title;
        public static PS3API PS3 = new PS3API();
        public static Point newpoint = new Point();
        public static byte[] theme_color, shader_alpha;
        #endregion
        #region "Form Initialize Component"
        public Form1()
        {
            InitializeComponent();
            title = Text;
        }
        #endregion
        #region "Form Load"
        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = 0;
            shader_alpha = new byte[] { 0xFF, 0x00, 0x00, 0x00 };
            theme_color = new byte[] { 0x1F, 0x3F, 0x00, 0x00, 0x3F, 0x50, 0x00, 0x00, 0x3F, 0xF0, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC };
        }
        #endregion
        #region "Form Controls"
        private void button1_Click(object sender, EventArgs e)
        {
            if (!label2.Text.Contains("Not Attached"))
            {
                menu.Stop();
                PS3.SetMemory(0x31d9a5c0, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80 }); //hint shader size
                PS3.SetMemory(0x31d9a360, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00 }); //FJ_Label_SmallWhite Size and pos
                closeMenu();
                closeMenu();
            }
            Environment.Exit(0);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void xMouseDown(object sender, MouseEventArgs e)
        {
            x = Control.MousePosition.X - Location.X;
            y = Control.MousePosition.Y - Location.Y;
        }
        private void xMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                newpoint = Control.MousePosition;
                newpoint.X -= x;
                newpoint.Y -= y;
                Location = newpoint;
            }
        }
        const Int32 WS_MINIMIZEBOX = 0x20000, CS_DBLCLKS = 0x8;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }
        #endregion
        #region "API Changer"
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (listBox1.SelectedIndex)
            {
                case 0: PS3.ChangeAPI(SelectAPI.ControlConsole); break;
                case 1: PS3.ChangeAPI(SelectAPI.TargetManager); break;
            }
        }
        #endregion
        #region "Connect To PS3"
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (!PS3.ConnectTarget()) label2.Text = "Status: Not Connected || Not Attached";
                else label2.Text = "Status: Connected || Not Attached";
            }
            catch (Exception)
            {
                label2.Text = "Status: Not Connected || Not Attached";
            }
        }
        #endregion
        #region "Attach Game Process"
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (!label2.Text.Contains("Not Connected"))
                    if (!PS3.AttachProcess()) label2.Text = "Status: Connected || Not Attached";
                    else
                    {
                        label2.Text = "Status: Connected || Attached";
                        PS3.SetMemory(0x31d9ab91, Encoding.ASCII.GetBytes(fixText("Menu Base\0\0\0")));
                    }
                else label2.Text = "Status: Not Connected || Not Attached";
            }
            catch (Exception)
            {
                label2.Text = "Status: Connected || Not Attached";
            }
        }
        #endregion
        #region "Functions"

        #region "Notify Message"
        private void doNotify(string putDots)
        {//white text 0x31d9ab91 - pos size 0x31d9a820 // smallwhite text 0x31D9B091 - pos size 0x31d9a360
            PS3.SetMemory(0x31D9B091, Encoding.ASCII.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0"));
            PS3.SetMemory(0x31D9B091, Encoding.ASCII.GetBytes(fixText(putDots) + "\0" + "\0"));
            PS3.SetMemory(0x31d9a5c0, new byte[] { 0x3F, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x20 }); //hint shader size
            PS3.SetMemory(0x31d9a360, new byte[] { 0x3F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0xF0, 0x00, 0x00, 0x42, 0x80, 0x00, 0x00, 0x42, 0xE0, 0x00 }); //FJ_Label_SmallWhite Size and pos
            notifyTimer.Stop();
            notifyTimer.Start();
        }
        private void notifyTimer_Tick(object sender, EventArgs e)
        {
            notifyTimer.Stop();
            PS3.SetMemory(0x31d9a5c0, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80 }); //hint shader size
            PS3.SetMemory(0x31d9a360, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00 }); //FJ_Label_SmallWhite Size and pos
        }
        #endregion
        #region "Button Monitoring"
        public static class MCbuttonsOffsets
        {
            public static uint
                R1 = 0xB3,
                R2 = 0xB1,
                R3 = 0xB2,
                L1 = 0xB2,
                L2 = 0xB1,
                L3 = 0xB2,
                DpadUp = 0xB2,
                DpadDown = 0xB2,
                DpadLeft = 0xB2,
                DpadRight = 0xB2,
                Cross = 0xB3,
                Square = 0xB3,
                Circle = 0xB3,
                Triangle = 0xB3,
                Select = 0xB3,
                Start = 0xB3;
        }
        public static class MCbuttonsBytes
        {
            public static byte
                R1 = 0x40,
                R2 = 0x40,
                R3 = 0x01,
                L1 = 0x80,
                L2 = 0x80,
                L3 = 0x02,
                DpadUp = 0x04,
                DpadDown = 0x08,
                DpadLeft = 0x10,
                DpadRight = 0x20,
                Cross = 0x01,
                Square = 0x04,
                Circle = 0x02,
                Triangle = 0x08,
                Select = 0x20,
                Start = 0x10;
        }
        public static bool MCbuttonPressed(uint MCoffset, byte Mcbyte)
        {
            if (PS3.Extension.ReadByte(0x3000c128 + MCoffset) == Mcbyte)
                return true;
            else
                return false;
        }
        #endregion

        #endregion
        #region "Mod Menu"

        #region "Menu Variables"
        string currentMenu = "main";
        bool inMenu = false, check = true;
        byte scrollerAlphaByte1 = 0x3F, scrollerAlphaByte2 = 0xFF;
        int max = 2, scrollPos = 0, switchInt = 0, alphaScrollerPos = 1440;
        #endregion
        #region "Fix Text"
        private string fixText(string fixStr)
        {
            int getPos = fixStr.Length;
            int place = 1;
            for (int i = 0; i < getPos; i++)
            {
                fixStr = fixStr.Insert(place, ".");
                place += 2;
            }
            fixStr = fixStr.Replace(".", "\0");
            return fixStr + "\0\0";
        }
        #endregion
        #region "Hud and Text"
        private void textOn()
        {
            PS3.SetMemory(0x31736B41, Encoding.ASCII.GetBytes(fixText("Crafting     Inventory\n\n\n\n\n\n\n\nOption 1\nOption 2\nOption 3\nOption 4\nOption 5\nOption 6\nOption 7\nOption 8\nOption 9\nOption 10\nOption 11\nOption 12\nOption 13\nOption 14\nSub Menu 3\n\n\n\n\n\nOption 1\nOption 2\nOption 3\nOption 4\nOption 5\nOption 6\nOption 7\nOption 8\nOption 9\nOption 10\nOption 11\nOption 12\nOption 13\nOption 14\nOption 15\n\n\n\n\n\n\n\n\nOption 1\nOption 2\nOption 3\nOption 4\nOption 5\nOption 6\n\n\n\n\n\n\n\n\n\n\n\n\nSub Menu 1\nSub Menu 2\nToggle Theme Color\nToggle Shader Alpha")));
        }
        private void textOff()
        {
            PS3.SetMemory(0x31736B41, new byte[] { 0x43, 0x00, 0x72, 0x00, 0x61, 0x00, 0x66, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x67, 0x00, 0x00, 0x00, 0x73, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x65, 0x00, 0x63, 0x00, 0x74, 0x00, 0x65, 0x00, 0x64, 0x00, 0x2C, 0x00, 0x20, 0x00, 0x79, 0x00, 0x6F, 0x00, 0x75, 0x00, 0x00, 0x00, 0x33, 0x31, 0x34, 0xB0, 0xA6, 0x00, 0x00, 0x00, 0x28, 0x31, 0x73, 0x6C, 0x20, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00, 0x00, 0x31, 0x34, 0xB0, 0xA6, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x33, 0x69, 0xC7, 0x8F, 0x9D, 0x00, 0x00, 0x00, 0x28, 0x00, 0x50, 0x00, 0x61, 0x00, 0x75, 0x00, 0x73, 0x00, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x07, 0x31, 0x74, 0x7E, 0x20, 0x69, 0xC7, 0x8F, 0x9D, 0x00, 0x67, 0x00, 0x29, 0x00, 0x00, 0x00, 0x33, 0x00, 0x4C, 0x00, 0x61, 0x00, 0x79, 0x00, 0x6F, 0x00, 0x75, 0x00, 0x74, 0x00, 0x20, 0x00, 0x32, 0x00, 0x00, 0x00, 0x6E, 0x00, 0x76, 0x00, 0x69, 0x00, 0x74, 0x00, 0x65, 0x00, 0x00, 0x00, 0x74, 0x00, 0x20, 0x00, 0x28, 0x00, 0x57, 0x00, 0x68, 0x00, 0x65, 0x00, 0x6E, 0x00, 0x00, 0x00, 0x23, 0x00, 0x00, 0x20, 0x14, 0x01, 0x0B, 0x00, 0x10, 0x31, 0x78, 0xB7, 0xD8, 0x00, 0x00, 0x20, 0x14, 0x00, 0x0D, 0x00, 0x0A, 0x00, 0x55, 0x00, 0x6E, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x43, 0x00, 0x4D, 0x00, 0x6F, 0x00, 0x76, 0x00, 0x65, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x20, 0x00, 0x28, 0x00, 0x57, 0x00, 0x68, 0x00, 0x65, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x46, 0x00, 0x6C, 0x00, 0x79, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x67, 0x00, 0x29, 0x00, 0x00, 0x00, 0x6B, 0x00, 0x20, 0x00, 0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x20, 0x00, 0x79, 0x00, 0x00, 0x00, 0x33, 0x7A, 0x6E, 0xAD, 0x8E, 0x00, 0x00, 0x00, 0x28, 0x00, 0x44, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x70, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x07, 0x31, 0x76, 0xCE, 0xD0, 0x7A, 0x6E, 0xAD, 0x8E, 0x00, 0x20, 0x00, 0x75, 0x00, 0x00, 0x00, 0x33, 0x3F, 0xEF, 0x33, 0x4D, 0x00, 0x00, 0x00, 0x28, 0x00, 0x4A, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x70, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x07, 0x31, 0x73, 0x0C, 0x30, 0x3F, 0xEF, 0x33, 0x4D, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x33, 0xC2, 0x69, 0x5D, 0x1D, 0x00, 0x00, 0x00, 0x28, 0x31, 0x73, 0x6A, 0xB0, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x0F, 0x31, 0x70, 0x43, 0x30, 0xC2, 0x69, 0x5D, 0x1D, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x33, 0x74, 0x45, 0x3D, 0x47, 0x00, 0x00, 0x00, 0x28, 0x31, 0x73, 0x6D, 0x20, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x0F, 0x31, 0x70, 0x31, 0x40, 0x74, 0x45, 0x3D, 0x47, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0x00, 0x43, 0x00, 0x79, 0x00, 0x63, 0x00, 0x6C, 0x00, 0x65, 0x00, 0x20, 0x00, 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x64, 0x00, 0x20, 0x00, 0x49, 0x00, 0x74, 0x00, 0x65, 0x00, 0x6D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x13, 0x31, 0x73, 0xDF, 0xA0, 0x31, 0x72, 0xF6, 0x40, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x01, 0x00, 0x10, 0x00, 0x00, 0x31, 0x73, 0x7E, 0x78, 0x00, 0x00, 0x00, 0x01, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0xAE, 0x00, 0x33, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x33, 0x85, 0x34, 0x79, 0xB8, 0x00, 0x00, 0x00, 0x28, 0x31, 0x73, 0x6D, 0xB0, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0F, 0x31, 0x76, 0xAC, 0x80, 0x85, 0x34, 0x79, 0xB8, 0x00, 0x20, 0x00, 0x61, 0x00, 0x00, 0x00, 0x33, 0x00, 0x42, 0x00, 0x61, 0x00, 0x6E, 0x00, 0x6E, 0x00, 0x65, 0x00, 0x64, 0x00, 0x20, 0x00, 0x4C, 0x00, 0x65, 0x00, 0x76, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x00, 0x00, 0x69, 0x00, 0x63, 0x00, 0x20, 0x00, 0x66, 0x00, 0x65, 0x00, 0x61, 0x00, 0x74, 0x00, 0x75, 0x00, 0x72, 0x00, 0x00, 0x00, 0x33, 0xD3, 0xE3, 0xAC, 0xFB, 0x00, 0x00, 0x00, 0x28, 0x00, 0x42, 0x00, 0x61, 0x00, 0x74, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x07, 0x31, 0x72, 0x76, 0x90, 0xD3, 0xE3, 0xAC, 0xFB, 0x00, 0x66, 0x00, 0x74, 0x00, 0x00, 0x00, 0x13, 0x01, 0x25, 0xAE, 0x50, 0x01, 0x25, 0x0D, 0x78, 0x31, 0xEE, 0xE7, 0xD0, 0x00, 0x00, 0x00, 0x33, 0xD9, 0xD1, 0xB1, 0x15, 0x00, 0x00, 0x00, 0x28, 0x30, 0x2E, 0x80, 0x60, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x47, 0x00, 0x00, 0x00, 0x47, 0x31, 0x74, 0xA5, 0x90, 0xD9, 0xD1, 0xB1, 0x15, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x40, 0x00, 0x31, 0x73, 0x62, 0x38, 0x00, 0x00, 0x00, 0x03, 0x00, 0x65, 0x00, 0x6E, 0x00, 0x63, 0x00, 0x65, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x01, 0x33, 0x00, 0x43, 0x00, 0x68, 0x00, 0x61, 0x00, 0x74, 0x00, 0x20, 0x00, 0x69, 0x00, 0x73, 0x00, 0x20, 0x00, 0x64, 0x00, 0x69, 0x00, 0x73, 0x00, 0x61, 0x00, 0x62, 0x00, 0x6C, 0x00, 0x65, 0x00, 0x64, 0x00, 0x20, 0x00, 0x61, 0x00, 0x73, 0x00, 0x20, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x65, 0x00, 0x20, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x20, 0x00, 0x6D, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x65, 0x00, 0x20, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x63, 0x00, 0x61, 0x00, 0x6C, 0x00, 0x20, 0x00, 0x70, 0x00, 0x6C, 0x00, 0x61, 0x00, 0x79, 0x00, 0x65, 0x00, 0x72, 0x00, 0x73, 0x00, 0x20, 0x00, 0x61, 0x00, 0x72, 0x00, 0x65, 0x00, 0x20, 0x00, 0x6E, 0x00, 0x6F, 0x00, 0x74, 0x00, 0x20, 0x00, 0x61, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x77, 0x00, 0x65, 0x00, 0x64, 0x00, 0x20, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x20, 0x00, 0x75, 0x00, 0x73, 0x00, 0x65, 0x00, 0x20, 0x00, 0x63, 0x00, 0x68, 0x00, 0x61, 0x00, 0x74, 0x00, 0x20, 0x00, 0x64, 0x00, 0x75, 0x00, 0x65, 0x00, 0x20, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x20, 0x00, 0x74, 0x00, 0x68, 0x00, 0x65, 0x00, 0x69, 0x00, 0x72, 0x00, 0x20, 0x00, 0x53, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x79, 0x00, 0x20, 0x00, 0x45, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00, 0x72, 0x00, 0x74, 0x00, 0x61, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x20, 0x00, 0x4E, 0x00, 0x65, 0x00, 0x74, 0x00, 0x77, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6B, 0x00, 0x20, 0x00, 0x61, 0x00, 0x63, 0x00, 0x63, 0x00, 0x6F, 0x00, 0x75, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x20, 0x00, 0x63, 0x00, 0x68, 0x00, 0x61, 0x00, 0x74, 0x00, 0x20, 0x00, 0x72, 0x00, 0x65, 0x00, 0x73, 0x00, 0x74, 0x00, 0x72, 0x00, 0x69, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x73, 0x00, 0x2E, 0x00, 0x00, 0x00, 0x72, 0x00, 0x69, 0x00, 0x61, 0x00, 0x6E, 0x00, 0x00, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00, 0x2C, 0x00, 0x00, 0x00, 0x73, 0x00, 0x49, 0x00, 0x6E, 0x00, 0x70, 0x00, 0x75, 0x00, 0x74, 0x00, 0x20, 0x00, 0x74, 0x00, 0x68, 0x00, 0x65, 0x00, 0x20, 0x00, 0x73, 0x00, 0x65, 0x00, 0x65, 0x00, 0x64, 0x00, 0x20, 0x00, 0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x20, 0x00, 0x79, 0x00, 0x6F, 0x00, 0x75, 0x00, 0x72, 0x00, 0x20, 0x00, 0x77, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6C, 0x00, 0x64, 0x00, 0x20, 0x00, 0x67, 0x00, 0x65, 0x00, 0x6E, 0x00, 0x65, 0x00, 0x72, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E });
        }
        private void hudOn()
        {
            //Using Inv inv/craft Scroller size pos updated Inv Shader 0x320DA388
            PS3.SetMemory(0x320DA388, new byte[] { 0x41, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, scrollerAlphaByte1, scrollerAlphaByte2, 0x00, 0x00, 0x43, 0x20, 0x00, 0x00, 0x45 });
            PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(alphaScrollerPos + scrollPos * 501));
            //Text size stick
            PS3.SetMemory(0x31b32708, new byte[] { 0x3F, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0xA0 });
            //Using Craft inv/craft Text size pos updated  0x320da4b8 - Inv Text size pos - 0x31b325d8 craft Text size pos  last to first 00 00 00     44 0D 00      44 A0 D0      44 A0 00       44 EF 20 
            PS3.SetMemory(0x31b325d8, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x43, 0xDC, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //Using Craft inv/craft shader size pos //        updated Craft Shader 0x31b324a8
            PS3.SetMemory(0x31b324a8, new byte[] { 0x41, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0xDF, 0x00, 0x00, 0x43, 0xDC, 0x00, 0x00, 0x40, 0x00, 0x00 });
            // Menu Color
            PS3.SetMemory(0x31B2CF70, theme_color);
            //FJlabelwhite size pos
            PS3.SetMemory(0x31d9a6f0, new byte[] { 0x3F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0xF0, 0x00, 0x00, 0x43, 0xE5, 0x00, 0x00, 0x43, 0x00, 0x00 });
            // Shader Fix new
            PS3.SetMemory(0x31391B50, shader_alpha);
            // Scroll bar Fix new
            PS3.SetMemory(0x3138E520, new byte[] { 0xFF, 0x00 });
        }
        private void hudOff()
        {
            //Text size stick
            PS3.SetMemory(0x31b32708, new byte[] { 0x3F, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80 });
            //Using Craft inv/craft Text size pos updated  0x320da4b8 - Inv Text size pos - 0x31b325d8 craft Text size pos
            PS3.SetMemory(0x31b325d8, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x41, 0xF8, 0x00, 0x00, 0x41, 0x10, 0x00 });
            //Using Craft inv/craft shader size pos // last to first 00 00 00     44 0D 00      44 A0 D0      44 A0 00       44 EF 20         updated Craft Shader 0x31b324a8
            PS3.SetMemory(0x31b324a8, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //Using Inv inv/craft Scroller size pos updated Inv Shader 0x320DA388
            PS3.SetMemory(0x320DA388, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            System.Threading.Tasks.Task.Delay(150).Wait();
            // Menu Color
            PS3.SetMemory(0x31B2CF70, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC });
            //FJlabelwhite size pos
            PS3.SetMemory(0x31d9a6f0, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            // Shader Fix new
            PS3.SetMemory(0x31391B50, new byte[] { 0x3F, 0x80 });
            // Scroll bar Fix new
            PS3.SetMemory(0x3138E520, new byte[] { 0x3F, 0x80 });
        }
        #endregion
        #region "Process Functions"
        private void openMenu()
        {
            hudOn();
            PS3.SetMemory(0x015B1885, new byte[] { 0x01 }); // lock controls
            inMenu = true;
        }
        private void closeMenu()
        {
            switch (currentMenu)
            {
                case "main":
                    {
                        hudOff();
                        textOff();
                        PS3.SetMemory(0x015B1885, new byte[] { 0x00 }); // lock controls
                        PS3.SetMemory(0x31F0FC24, new byte[] { 0x3F, 0x80, 0x00 }); // move text up
                        inMenu = false;
                        check = true;
                        max = 3;
                        break;
                    }
                case "host":
                    {
                        currentMenu = "main";
                        max = 3;
                        scrollPos = 0;
                        PS3.SetMemory(0x31b325ec, new byte[] { 0x00, 0x00, 0x00 });
                        PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(1440 + 0 * 501));
                        break;
                    }
                case "nonHost1":
                    {
                        currentMenu = "main";
                        max = 3;
                        scrollPos = 1;
                        PS3.SetMemory(0x31b325ec, new byte[] { 0x00, 0x00, 0x00 });
                        PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(1440 + 1 * 501));
                        break;
                    }
                case "nonHost2":
                    {
                        currentMenu = "nonHost1";
                        max = 3;
                        scrollPos = 0;
                        PS3.SetMemory(0x31b325ec, new byte[] { 0x44, 0xEF, 0x20 });
                        PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(1440 + 14 * 501));
                        break;
                    }
            }
        }
        private void SelectOpt()
        {
            switch (currentMenu)
            {
                case "main":
                    {
                        switch (scrollPos)
                        {
                            case 0:
                                {
                                    currentMenu = "host";
                                    max = 5;
                                    scrollPos = 0;
                                    PS3.SetMemory(0x31b325ec, new byte[] { 0x44, 0x0D, 0x00 });
                                    PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(1440));
                                    break;
                                }
                            case 1:
                                {
                                    currentMenu = "nonHost1";
                                    max = 14;
                                    scrollPos = 0;
                                    PS3.SetMemory(0x31b325ec, new byte[] { 0x44, 0xEF, 0x20 });
                                    PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(1440 + 0 * 501));
                                    break;
                                }
                            case 2: menuTheme(); break;
                            case 3: alphaShader(); break;
                        }
                        break;
                    }
                case "host":
                    {
                        switch (scrollPos)
                        {
                            case 0: SubMenuOpt1(); break;
                            case 1: SubMenuOpt2(); break;
                            case 2: SubMenuOpt3(); break;
                            case 3: SubMenuOpt4(); break;
                            case 4: SubMenuOpt5(); break;
                            case 5: SubMenuOpt6(); break;
                        }
                        break;
                    }
                case "nonHost1":
                    {
                        switch (scrollPos)
                        {
                            case 0: Opt1x1(); break;
                            case 1: Opt1x2(); break;
                            case 2: Opt1x3(); break;
                            case 3: Opt1x4(); break;
                            case 4: Opt1x5(); break;
                            case 5: Opt1x6(); break;
                            case 6: Opt1x7(); break;
                            case 7: Opt1x8(); break;
                            case 8: Opt1x9(); break;
                            case 9: Opt1x10(); break;
                            case 10: Opt1x11(); break;
                            case 11: Opt1x12(); break;
                            case 12: Opt1x13(); break;
                            case 13: Opt1x14(); break;
                            case 14:
                                {
                                    currentMenu = "nonHost2";
                                    max = 14;
                                    scrollPos = 0;
                                    PS3.SetMemory(0x31b325ec, new byte[] { 0x44, 0xA0, 0xD0 });
                                    PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(1440 + 0 * 501)); break;
                                }
                        }
                        break;
                    }
                case "nonHost2":
                    {
                        switch (scrollPos)
                        {
                            case 0: Opt2x1(); break;
                            case 1: Opt2x2(); break;
                            case 2: Opt2x3(); break;
                            case 3: Opt2x4(); break;
                            case 4: Opt2x5(); break;
                            case 5: Opt2x6(); break;
                            case 6: Opt2x7(); break;
                            case 7: Opt2x8(); break;
                            case 8: Opt2x9(); break;
                            case 9: Opt2x10(); break;
                            case 10: Opt2x11(); break;
                            case 11: Opt2x12(); break;
                            case 12: Opt2x13(); break;
                            case 13: Opt2x14(); break;
                            case 14: Opt2x15(); break;
                        }
                        break;
                    }
            }
        }
        #endregion

        #endregion
        #region "Options"

        #region "Toggle Alpha"
        int intAlphaShader = 1;
        byte[] alphaShaderBytes = { 0xFF, 0x00, 0x00, 0x00 };
        private void alphaShader()
        {
            switch (intAlphaShader)
            {
                case 1: alphaShaderBytes = new byte[] { 0x00, 0x00, 0x00, 0x00 }; intAlphaShader++; doNotify("Light Shader Alpha"); break;
                case 2: alphaShaderBytes = new byte[] { 0xFF, 0x00, 0x00, 0x00 }; intAlphaShader = 1; doNotify("Dark Shader Alpha"); break;
            }
            PS3.SetMemory(0x31391B50, alphaShaderBytes);
        }
        #endregion
        #region "Toggle Theme"
        int intOptTheme = 1;
        private void menuTheme()
        {
            switch (intOptTheme)
            {
                case 1: themeRed(); intOptTheme++; break;
                case 2: themeOrange(); intOptTheme++; break;
                case 3: themeYellow(); intOptTheme++; break;
                case 4: themeGreen(); intOptTheme++; break;
                case 5: themePurple(); intOptTheme++; break;
                case 6: themePink(); intOptTheme++; break;
                case 7: themeDefault(); intOptTheme = 1; break;
            }
        }
        private void themeDefault()
        {
            theme_color = new byte[] { 0x1F, 0x3F, 0x00, 0x00, 0x3F, 0x50, 0x00, 0x00, 0x3F, 0xF0, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC };
            PS3.SetMemory(0x31B2CF70, theme_color);
            doNotify("Blue Theme");
        }
        private void themeRed()
        {
            theme_color = new byte[] { 0x3F, 0xCF, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x0F, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC };
            PS3.SetMemory(0x31B2CF70, theme_color);
            doNotify("Red Theme");
        }
        private void themeOrange()
        {
            theme_color = new byte[] { 0x3F, 0xD3, 0x00, 0x00, 0x3F, 0x54, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC };
            PS3.SetMemory(0x31B2CF70, theme_color);
            doNotify("Orange Theme");
        }
        private void themeYellow()
        {
            theme_color = new byte[] { 0x3F, 0xF7, 0x00, 0x00, 0x3F, 0xCA, 0x00, 0x00, 0x3F, 0x18, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC };
            PS3.SetMemory(0x31B2CF70, theme_color);
            doNotify("Yellow Theme");
        }
        private void themeGreen()
        {
            theme_color = new byte[] { 0x3F, 0x26, 0x00, 0x00, 0x3F, 0xA6, 0x00, 0x00, 0x3F, 0x5B, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC };
            PS3.SetMemory(0x31B2CF70, theme_color);
            doNotify("Green Theme");
        }
        private void themePurple()
        {
            theme_color = new byte[] { 0x3F, 0x9A, 0x00, 0x00, 0x3F, 0x12, 0x00, 0x00, 0x3F, 0xB3, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC };
            PS3.SetMemory(0x31B2CF70, theme_color);
            doNotify("Purple Theme");
        }
        private void themePink()
        {
            theme_color = new byte[] { 0x3F, 0xF6, 0x00, 0x00, 0x3F, 0x24, 0x00, 0x00, 0x3F, 0x59, 0x00, 0x00, 0x3F, 0x30, 0xFF, 0xFC };
            PS3.SetMemory(0x31B2CF70, theme_color);
            doNotify("Pink Theme");
        }
        #endregion
        #region "Host Options"
        int intsubmenuopt1 = 1, intsubmenuopt2 = 1, intsubmenuopt3 = 1, intsubmenuopt4 = 1, intsubmenuopt5 = 1, intsubmenuopt6 = 1;
        private void SubMenuOpt1()
        {
            switch (intsubmenuopt1)
            {
                case 1: doNotify("Option 1: On"); intsubmenuopt1++; break;
                case 2: doNotify("Option 1: Off"); intsubmenuopt1 = 1; break;
            }
        }
        private void SubMenuOpt2()
        {
            switch (intsubmenuopt2)
            {
                case 1: doNotify("Option 2: On"); intsubmenuopt2++; break;
                case 2: doNotify("Option 2: Off"); intsubmenuopt2 = 1; break;
            }
        }
        private void SubMenuOpt3()
        {
            switch (intsubmenuopt3)
            {
                case 1: doNotify("Option 3: On"); intsubmenuopt3++; break;
                case 2: doNotify("Option 3: Off"); intsubmenuopt3 = 1; break;
            }
        }
        private void SubMenuOpt4()
        {
            switch (intsubmenuopt4)
            {
                case 1: doNotify("Option 4: On"); intsubmenuopt4++; break;
                case 2: doNotify("Option 4: Off"); intsubmenuopt4 = 1; break;
            }
        }
        private void SubMenuOpt5()
        {
            switch (intsubmenuopt5)
            {
                case 1: doNotify("Option 5: On"); intsubmenuopt5++; break;
                case 2: doNotify("Option 5: Off"); intsubmenuopt5 = 1; break;
            }
        }
        private void SubMenuOpt6()
        {
            switch (intsubmenuopt6)
            {
                case 1: doNotify("Option 6: On"); intsubmenuopt6++; break;
                case 2: doNotify("Option 6: Off"); intsubmenuopt6 = 1; break;
            }
        }
        #endregion
        #region "Non Host Options"
        int intOpt1x1 = 1, intOpt1x2 = 1, intOpt1x3 = 1, intOpt1x4 = 1, intOpt1x5 = 1, intOpt1x6 = 1, intOpt1x7 = 1, intOpt1x8 = 1, intOpt1x9 = 1, intOpt1x10 = 1, intOpt1x11 = 1, intOpt1x12 = 1, intOpt1x13 = 1, intOpt1x14 = 1;
        private void Opt1x1()
        {
            switch (intOpt1x1)
            {
                case 1: doNotify("Option 1: On"); intOpt1x1++; break;
                case 2: doNotify("Option 1: Off"); intOpt1x1 = 1; break;
            }
        }
        private void Opt1x2()
        {
            switch (intOpt1x2)
            {
                case 1: doNotify("Option 2: On"); intOpt1x2++; break;
                case 2: doNotify("Option 2: Off"); intOpt1x2 = 1; break;
            }
        }
        private void Opt1x3()
        {
            switch (intOpt1x3)
            {
                case 1: doNotify("Option 3: On"); intOpt1x3++; break;
                case 2: doNotify("Option 3: Off"); intOpt1x3 = 1; break;
            }
        }
        private void Opt1x4()
        {
            switch (intOpt1x4)
            {
                case 1: doNotify("Option 4: On"); intOpt1x4++; break;
                case 2: doNotify("Option 4: Off"); intOpt1x4 = 1; break;
            }
        }
        private void Opt1x5()
        {
            switch (intOpt1x5)
            {
                case 1: doNotify("Option 5: On"); intOpt1x5++; break;
                case 2: doNotify("Option 5: Off"); intOpt1x5 = 1; break;
            }
        }
        private void Opt1x6()
        {
            switch (intOpt1x6)
            {
                case 1: doNotify("Option 6: On"); intOpt1x6++; break;
                case 2: doNotify("Option 6: Off"); intOpt1x6 = 1; break;
            }
        }
        private void Opt1x7()
        {
            switch (intOpt1x7)
            {
                case 1: doNotify("Option 7: On"); intOpt1x7++; break;
                case 2: doNotify("Option 7: Off"); intOpt1x7 = 1; break;
            }
        }
        private void Opt1x8()
        {
            switch (intOpt1x8)
            {
                case 1: doNotify("Option 8: On"); intOpt1x8++; break;
                case 2: doNotify("Option 8: Off"); intOpt1x8 = 1; break;
            }
        }
        private void Opt1x9()
        {
            switch (intOpt1x9)
            {
                case 1: doNotify("Option 9: On"); intOpt1x9++; break;
                case 2: doNotify("Option 9: Off"); intOpt1x9 = 1; break;
            }
        }
        private void Opt1x10()
        {
            switch (intOpt1x10)
            {
                case 1: doNotify("Option 10: On"); intOpt1x10++; break;
                case 2: doNotify("Option 10: Off"); intOpt1x10 = 1; break;
            }
        }
        private void Opt1x11()
        {
            switch (intOpt1x11)
            {
                case 1: doNotify("Option 11: On"); intOpt1x11++; break;
                case 2: doNotify("Option 11: Off"); intOpt1x11 = 1; break;
            }
        }
        private void Opt1x12()
        {
            switch (intOpt1x12)
            {
                case 1: doNotify("Option 12: On"); intOpt1x12++; break;
                case 2: doNotify("Option 12: Off"); intOpt1x12 = 1; break;
            }
        }
        private void Opt1x13()
        {
            switch (intOpt1x13)
            {
                case 1: doNotify("Option 13: On"); intOpt1x13++; break;
                case 2: doNotify("Option 13: Off"); intOpt1x13 = 1; break;
            }
        }
        private void Opt1x14()
        {
            switch (intOpt1x14)
            {
                case 1: doNotify("Option 14: On"); intOpt1x14++; break;
                case 2: doNotify("Option 14: Off"); intOpt1x14 = 1; break;
            }
        }
        #endregion
        #region "Sub Menu 3 (Inside Sub Menu 2)"
        int intOpt2x1 = 1, intOpt2x2 = 1, intOpt2x3 = 1, intOpt2x4 = 1, intOpt2x5 = 1, intOpt2x6 = 1, intOpt2x7 = 1, intOpt2x8 = 1, intOpt2x9 = 1, intOpt2x10 = 1, intOpt2x11 = 1, intOpt2x12 = 1, intOpt2x13 = 1, intOpt2x14 = 1, intOpt2x15 = 1;
        private void Opt2x1()
        {
            switch (intOpt2x1)
            {
                case 1: doNotify("Option 1: On"); intOpt2x1++; break;
                case 2: doNotify("Option 1: Off"); intOpt2x1 = 1; break;
            }
        }
        private void Opt2x2()
        {
            switch (intOpt2x2)
            {
                case 1: doNotify("Option 2: On"); intOpt2x2++; break;
                case 2: doNotify("Option 2: Off"); intOpt2x2 = 1; break;
            }
        }
        private void Opt2x3()
        {
            switch (intOpt2x3)
            {
                case 1: doNotify("Option 3: On"); intOpt2x3++; break;
                case 2: doNotify("Option 3: Off"); intOpt2x3 = 1; break;
            }
        }
        private void Opt2x4()
        {
            switch (intOpt2x4)
            {
                case 1: doNotify("Option 4: On"); intOpt2x4++; break;
                case 2: doNotify("Option 4: Off"); intOpt2x4 = 1; break;
            }
        }
        private void Opt2x5()
        {
            switch (intOpt2x5)
            {
                case 1: doNotify("Option 5: On"); intOpt2x5++; break;
                case 2: doNotify("Option 5: Off"); intOpt2x5 = 1; break;
            }
        }
        private void Opt2x6()
        {
            switch (intOpt2x6)
            {
                case 1: doNotify("Option 6: On"); intOpt2x6++; break;
                case 2: doNotify("Option 6: Off"); intOpt2x6 = 1; break;
            }
        }
        private void Opt2x7()
        {
            switch (intOpt2x7)
            {
                case 1: doNotify("Option 7: On"); intOpt2x7++; break;
                case 2: doNotify("Option 7: Off"); intOpt2x7 = 1; break;
            }
        }
        private void Opt2x8()
        {
            switch (intOpt2x8)
            {
                case 1: doNotify("Option 8: On"); intOpt2x8++; break;
                case 2: doNotify("Option 8: Off"); intOpt2x8 = 1; break;
            }
        }
        private void Opt2x9()
        {
            switch (intOpt2x9)
            {
                case 1: doNotify("Option 9: On"); intOpt2x9++; break;
                case 2: doNotify("Option 9: Off"); intOpt2x9 = 1; break;
            }
        }
        private void Opt2x10()
        {
            switch (intOpt2x10)
            {
                case 1: doNotify("Option 10: On"); intOpt2x10++; break;
                case 2: doNotify("Option 10: Off"); intOpt2x10 = 1; break;
            }
        }
        private void Opt2x11()
        {
            switch (intOpt2x11)
            {
                case 1: doNotify("Option 11: On"); intOpt2x11++; break;
                case 2: doNotify("Option 11: Off"); intOpt2x11 = 1; break;
            }
        }
        private void Opt2x12()
        {
            switch (intOpt2x12)
            {
                case 1: doNotify("Option 12: On"); intOpt2x12++; break;
                case 2: doNotify("Option 12: Off"); intOpt2x12 = 1; break;
            }
        }
        private void Opt2x13()
        {
            switch (intOpt2x13)
            {
                case 1: doNotify("Option 13: On"); intOpt2x13++; break;
                case 2: doNotify("Option 13: Off"); intOpt2x13 = 1; break;
            }
        }
        private void Opt2x14()
        {
            switch (intOpt2x14)
            {
                case 1: doNotify("Option 14: On"); intOpt2x14++; break;
                case 2: doNotify("Option 14: Off"); intOpt2x14 = 1; break;
            }
        }
        private void Opt2x15()
        {
            switch (intOpt2x15)
            {
                case 1: doNotify("Option 15: On"); intOpt2x15++; break;
                case 2: doNotify("Option 15: Off"); intOpt2x15 = 1; break;
            }
        }

        #endregion

        #endregion
        #region "Timers"
        private void menu_Tick(object sender, EventArgs e)
        {
            switch (inMenu)
            {
                case true:
                    {
                        switch (MCbuttonPressed(MCbuttonsOffsets.DpadUp, MCbuttonsBytes.DpadUp))
                        {
                            case true:
                                {
                                    scrollPos--;
                                    if (scrollPos == -1) { scrollPos = max; }
                                    PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(1440 + scrollPos * 501));
                                    System.Threading.Tasks.Task.Delay(200).Wait();
                                    break;
                                }
                        }
                        switch (MCbuttonPressed(MCbuttonsOffsets.DpadDown, MCbuttonsBytes.DpadDown))
                        {
                            case true:
                                {
                                    scrollPos++;
                                    if (scrollPos == max + 1) { scrollPos = 0; }
                                    PS3.Extension.WriteInt16(0x320da39d, Convert.ToInt16(1440 + scrollPos * 501));
                                    System.Threading.Tasks.Task.Delay(200).Wait();
                                    break;
                                }
                        }
                        switch (MCbuttonPressed(MCbuttonsOffsets.Circle, MCbuttonsBytes.Circle))
                        {
                            case true:
                                {
                                    closeMenu();
                                    System.Threading.Tasks.Task.Delay(200).Wait();
                                    break;
                                }
                        }
                        switch (MCbuttonPressed(MCbuttonsOffsets.Square, MCbuttonsBytes.Square))
                        {
                            case true:
                                {
                                    SelectOpt();
                                    System.Threading.Tasks.Task.Delay(200).Wait();
                                    break;
                                }
                        }
                        break;
                    }
                case false:
                    {
                        switch (MCbuttonPressed(MCbuttonsOffsets.Start, MCbuttonsBytes.Start))
                        {
                            case true:
                                {
                                    switch (switchInt)
                                    {
                                        case 0:
                                            {
                                                switchInt++;
                                                PS3.SetMemory(0x31F0FC24, new byte[] { 0x42, 0xA7, 0xA0 }); // move text up
                                                textOn();
                                                menu1.Start();
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case false:
                                {
                                    switch (switchInt)
                                    {
                                        case 1:
                                            {
                                                switchInt--;
                                                menu1.Stop();
                                                switch (check)
                                                {
                                                    case true:
                                                        {
                                                            PS3.SetMemory(0x31F0FC24, new byte[] { 0x3F, 0x80, 0x00 }); // move text up
                                                            textOff();
                                                        }
                                                        break;
                                                }
                                                break;
                                            }
                                    }
                                }
                                break;
                        }
                        break;
                    }
            }
        }
        private void menu1_Tick(object sender, EventArgs e)
        {
            switch (PS3.Extension.ReadByte(0x015B1885))
            {
                case 0x00:
                    {
                        check = false;
                        menu1.Stop();
                        openMenu();
                        break;
                    }
            }
        }
        #endregion
        #region "Start Menu"
        private void button29_Click(object sender, EventArgs e)
        {
            closeMenu();
            menu.Stop();
            menu.Start();
            button28.Enabled = true;
            button29.Enabled = false;
        }
        #endregion
        #region "Stop Menu"
        private void button28_Click(object sender, EventArgs e)
        {
            menu.Stop();
            closeMenu();
            closeMenu();
            button29.Enabled = true;
            button28.Enabled = false;
        }
        #endregion

    }
    #endregion

}
#endregion