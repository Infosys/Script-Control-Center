using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.UIAutomation.SEE;

namespace EditUseCase
{
    public partial class UseCaseEditor : UserControl
    {
        //public variables
        public UseCase UseCaseTobeEdited { get; set; }

        //private variables
        int activityIndex = -1;
        string _useCaseFilePath = "";

        public UseCaseEditor()
        {
            InitializeComponent();
        }

        public UseCaseEditor(UseCase useCase, string useCaseFilePath)
        {
            InitializeComponent();
            UseCaseTobeEdited = useCase;
            _useCaseFilePath = useCaseFilePath;
        }

        private void UseCaseEditor_Load(object sender, EventArgs e)
        {            
            if (UseCaseTobeEdited != null)
            {
                //create the activity nodes under the usecase
                List<TreeNode> children = new List<TreeNode>();
                UseCaseTobeEdited.Activities.ForEach(act =>
                {
                    TreeNode child = new TreeNode("Activity- " + act.Id);
                    child.Tag = act;
                    children.Add(child);
                });
                //create the parent node
                TreeNode parent = new TreeNode("Use Case- " + UseCaseTobeEdited.Name, children.ToArray());
                parent.Tag = UseCaseTobeEdited;
                tvUseCase.Nodes.Add(parent);
                tvUseCase.ExpandAll();
            }
        }

        private void tvUseCase_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MessageBox.Show(e.Node.Text);
            splitContainer1.Panel2.Controls.Clear();
            if (e.Node.Tag != null)
            {
                if (e.Node.Tag.GetType() == typeof(UseCase))
                {
                    UseCaseView view = new UseCaseView(e.Node.Tag as UseCase);
                    //dont set the dock style this will avoid scroll bars to appear
                    //view.Dock = DockStyle.Fill;
                    view.UseCaseChanged += new UseCaseView.UseCaseChangedEventHandler(view_UseCaseChanged);
                    splitContainer1.Panel2.Controls.Add(view);
                }
                else if (e.Node.Tag.GetType() == typeof(Activity))
                {
                    ActivityView view = new ActivityView(e.Node.Tag as Activity);
                    //dont set the dock style this will avoid scroll bars to appear
                    //view.Dock = DockStyle.Fill;
                    view.ActivityChanged += new ActivityView.ActivityChangedEventHandler(view_ActivityChanged);
                    splitContainer1.Panel2.Controls.Add(view);

                    //assign the activity index
                    activityIndex = e.Node.Index;
                }
            }
        }

        void view_ActivityChanged(ActivityView.ActivityChangedArgs e)
        {
            if (activityIndex >= 0)
            {
                UseCaseTobeEdited.Activities[activityIndex] = e.ChangedActivity;
                UpdateUseCaseFile();
            }
        }

        void view_UseCaseChanged(UseCaseView.UseCaseChangedArgs e)
        {
            UseCaseTobeEdited = e.ChangedUseCase;
            if (tvUseCase.SelectedNode.Tag.GetType() == typeof(UseCase))
            {
                tvUseCase.SelectedNode.Text = "Use Case- " + e.ChangedUseCase.Name;
                UpdateUseCaseFile();
            }
        }

        void UpdateUseCaseFile()
        {
            System.IO.File.WriteAllText(_useCaseFilePath, SerializeAndDeserialize.Serialize(UseCaseTobeEdited));
        }        
    }
}
