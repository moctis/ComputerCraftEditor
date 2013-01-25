using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ComputerCraftEditor
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    public partial class Form1 : Form
    {
        private string basePath;

        private IntPtr ptr;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
            var text = this.textBox1.Text;
            var file =  this.textBox2.Text;
            if (checkBox1.Checked) file = "/disk" + file;


            ActiveWindow("Minecraft");
            SendKeys("rm " + file + "\r");
            SendKeys("edit " + file + "\r");
            SendKeys(text);
            Save();

            this.button2_Click(sender, e);

            var node = this.ScanNode(this.textBox2.Text, this.treeView1.Nodes);
            if (node != null) node.ForeColor = Color.Gray;

        }

        private void ActiveWindow(string title)
        {
            ptr = User32.FindWindowByCaption(IntPtr.Zero, title);
            User32.SetForegroundWindow((int)ptr);
            Thread.Sleep(2000);
        }

        private void Save()
        {
            Keyboard.Simulate(Keys.LControlKey);
            Thread.Sleep(20);
            Keyboard.Simulate(Keys.Enter);
            Thread.Sleep(200);
            Keyboard.Simulate(Keys.LControlKey);
            Thread.Sleep(20);
            Keyboard.Simulate(Keys.Right);
            Thread.Sleep(20);
            Keyboard.Simulate(Keys.Enter);
            Thread.Sleep(500);
        }

        private void SendKeys(string text)
        {
            int indent = 0;
            bool increaseIndent = true;
            User32.SetForegroundWindow((int)ptr);
            foreach (var ch in text)
            {
                if (ch == ' ' && increaseIndent) indent += 1;
                else if (ch == '\t' && increaseIndent) indent += 2;
                else increaseIndent = false;

                if (ch == '\n')
                {
                    for (int i = 0; i < indent; i++)
                        Keyboard.Simulate(Keys.Back);
                    increaseIndent = true;
                    indent = 0;
                    User32.SetForegroundWindow((int)ptr);
                }  
                else
                    Keyboard.SimulateChar(ch);
                Thread.Sleep(20);
            }
            Thread.Sleep(500);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            basePath = @"D:\Games\Minecraft Thailand\data\.minecraft\saves\com\computer\0";
            ScanDirectory(basePath);
            this.treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);

            var watcher = new FileSystemWatcher(this.basePath);
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath.Remove(0, basePath.Length);
            path = path.Replace("\\", "/");
            var node = this.ScanNode(path, this.treeView1.Nodes);
            if (node != null) node.ForeColor = Color.White;
            Console.WriteLine(path); 
        }

        private TreeNode ScanNode(string path, TreeNodeCollection nodes)
        {
            foreach (var node in nodes.OfType<TreeNode>())
            {
                if (node.FullPath == path) return node;
                if (node.Nodes.Count > 0)
                {
                    var ret = this.ScanNode(path, node.Nodes);
                    if (ret != null) return ret;
                }
            }
            return null;
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.textBox2.Text = e.Node.FullPath;
            var path = this.basePath + "/" + e.Node.FullPath;
            var reader = new StreamReader(path);
            var text = reader.ReadToEnd();
            reader.Close();
            this.textBox1.Text = text;
        }

        private void ScanDirectory(string path, TreeNode root = null)
        {            
            var directoryInfo = new DirectoryInfo(path);
            if (root == null)
            {
                this.treeView1.Nodes.Clear();
                root = new TreeNode("");
                root.ForeColor = Color.Gray;
                this.treeView1.Nodes.Add(root);
            }

            foreach (var file in directoryInfo.EnumerateFiles())
            {
                var node = new TreeNode(file.Name);
                node.ForeColor = Color.Gray;
                if (!node.Text.StartsWith("."))
                    root.Nodes.Add(node);
            }

            foreach (var dir in directoryInfo.EnumerateDirectories())
            {
                var node = new TreeNode(dir.Name);
                node.ForeColor = Color.Gray;
                if (!node.Text.StartsWith("."))
                {
                    root.Nodes.Add(node);
                    ScanDirectory(dir.FullName, node);
                }
            }
            root.ExpandAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var text = this.textBox1.Text;
                var path = this.basePath + this.textBox2.Text;
                var info = new FileInfo(path);
                if (!info.Directory.Exists) info.Directory.Create();
                var writer = new StreamWriter(path);
                writer.Write(text);
                writer.Flush();
                writer.Close();

                var node = this.ScanNode(this.textBox2.Text, this.treeView1.Nodes);
                if (node != null) node.ForeColor = Color.Gray;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ScanDirectory(basePath);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar=='\t')
            {
                textBox1.SelectedText = "  ";
                e.Handled = true;
            }
        }
    }
}
