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

namespace Infosys.ATR.ScriptEditor
{
    public partial class UseCaseEditor : UserControl
    {
        //public variables
        public UseCase UseCaseTobeEdited { get; set; }

        //private variables
        int activityIndex = -1;
        string _useCaseFilePath = "";
        List<Activity> organizedAct = new List<Activity>();

        public UseCaseEditor()
        {
            InitializeComponent();
        }

        public UseCaseEditor(string useCaseFilePath)
        {
            InitializeComponent();
            //get the use case data from the file path
            UseCase useCase = SerializeAndDeserialize.Deserialize(System.IO.File.ReadAllText(useCaseFilePath), typeof(UseCase)) as UseCase;
            UseCaseTobeEdited = useCase;
            _useCaseFilePath = useCaseFilePath;
        }

        private void UseCaseEditor_Load(object sender, EventArgs e)
        {
            //populate and refresh the tree view
            if (UseCaseTobeEdited != null)
            {
                GetForeMostParents().ForEach(parent => OrganizeActivities(parent.Id));
                //OrganizeActivities(GetForeMostParent().Id);
                UseCaseTobeEdited.Activities = organizedAct;
                RefreshTreeView();
            }
            //register to the drag and drop related events
            tvUseCase.ItemDrag += new ItemDragEventHandler(tvUseCase_ItemDrag);
            tvUseCase.DragDrop += new DragEventHandler(tvUseCase_DragDrop);
            tvUseCase.DragEnter += new DragEventHandler(tvUseCase_DragEnter);
            //tvUseCase.MouseClick += new MouseEventHandler(tvUseCase_MouseClick);
        }

        void tvUseCase_MouseClick(object sender, MouseEventArgs e)
        {
            //select the concerned node
            Point loc = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            TreeNode node = ((TreeView)sender).GetNodeAt(loc);
            tvUseCase.SelectedNode = node;
        }

        void tvUseCase_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void tvUseCase_DragDrop(object sender, DragEventArgs e)
        {
            Point finalLoc = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            TreeNode nodeBeingMoved = (TreeNode)e.Data.GetData(typeof(TreeNode));
            TreeNode destNode = ((TreeView)sender).GetNodeAt(finalLoc);
            int finalIndex = -1;
            //check if the node being moved in the main use case node, then ignore
            if (nodeBeingMoved.Parent == null)
                return;

            if (destNode == null)
            {
                //i.e if tried to drop to a blank location, then add the mode to the last
                destNode = nodeBeingMoved.Parent.LastNode;
            }
            else if (destNode.Parent == null)
                return; //i.e. to avoid moving to the location of the parent use case node

            //select the parent- use case node by default
            tvUseCase.SelectedNode = nodeBeingMoved.Parent;

            finalIndex = destNode.Index;
            //to move a node, first remove and then only add
            nodeBeingMoved.Parent.Nodes.Remove(nodeBeingMoved);
            destNode.Parent.Nodes.Insert(finalIndex, nodeBeingMoved);

            //then update the use case and also the script file           

        }

        void tvUseCase_ItemDrag(object sender, ItemDragEventArgs e)
        {
            tvUseCase.DoDragDrop(e.Item, DragDropEffects.Move);
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
                    Activity act = e.Node.Tag as Activity;
                    ActivityView view = new ActivityView(act, UseCaseTobeEdited.Id);
                    //dont set the dock style this will avoid scroll bars to appear
                    //view.Dock = DockStyle.Fill;
                    view.ActivityChanged += new ActivityView.ActivityChangedEventHandler(view_ActivityChanged);
                    splitContainer1.Panel2.Controls.Add(view);

                    //assign the activity index
                    //activityIndex = e.Node.Index;
                    activityIndex = UseCaseTobeEdited.Activities.IndexOf(act);
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
            UseCaseTobeEdited.Activities = GetNewOrderedActivity();
            RefreshTreeView();
            //if (tvUseCase.SelectedNode.Tag.GetType() == typeof(UseCase))
            //{
            //    tvUseCase.SelectedNode.Text = "Use Case- " + e.ChangedUseCase.Name;
            //    UpdateUseCaseFile();
            //}
            UpdateUseCaseFile();
        }

        void UpdateUseCaseFile()
        {
            System.IO.File.WriteAllText(_useCaseFilePath, SerializeAndDeserialize.Serialize(UseCaseTobeEdited));
        }

        Activity GetForeMostParent()
        {
            Activity foreMostParent = new Activity();
            foreach (Activity act in UseCaseTobeEdited.Activities)
            {
                if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                {
                    if (string.IsNullOrEmpty(act.ParentId))
                    {
                        foreMostParent = act;
                        break;
                    }
                    else
                    {
                        Activity tempParentAct = UseCaseTobeEdited.Activities.Where(act2 => act2.Id == act.ParentId).FirstOrDefault();
                        if (tempParentAct == null)
                        {
                            foreMostParent = act;
                            break;
                        }
                    }
                }
            }
            organizedAct.Add(foreMostParent);
            return foreMostParent;
        }

        List<Activity> GetForeMostParents()
        {
            List<Activity> foreMostParents = new List<Activity>();
            foreach (Activity act in UseCaseTobeEdited.Activities)
            {
                if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                {
                    if (string.IsNullOrEmpty(act.ParentId))
                    {
                        foreMostParents.Add(act);
                    }
                    else
                    {
                        Activity tempParentAct = UseCaseTobeEdited.Activities.Where(act2 => act2.Id == act.ParentId).FirstOrDefault();
                        if (tempParentAct == null)
                        {
                            foreMostParents.Add(act);
                        }
                    }
                }
            }
            organizedAct.AddRange(foreMostParents);
            return foreMostParents;
        }

        void OrganizeActivities(string foremostActivityId)
        {
            List<Activity> validActs = UseCaseTobeEdited.Activities.Where(act => !string.IsNullOrEmpty(act.TargetApplication.ApplicationExe)).ToList();
            for (int i = 0; i < validActs.Count; i++)
            {
                if (foremostActivityId == "")
                    break;
                Activity nextParent = UseCaseTobeEdited.Activities.Where(act => act.ParentId == foremostActivityId).FirstOrDefault();
                if (nextParent != null)
                {
                    foremostActivityId = nextParent.Id;
                    organizedAct.Add(nextParent);
                }
                else
                    foremostActivityId = "";
            }
        }

        List<Activity> GetNewOrderedActivity()
        {
            List<Activity> acts = new List<Activity>();
            string tempParentId = "";
            foreach (TreeNode node in tvUseCase.Nodes[0].Nodes)
            {
                if (node.Tag.GetType() == typeof(Activity))
                {
                    Activity currentAct = node.Tag as Activity;
                    currentAct.ParentId = tempParentId;
                    tempParentId = currentAct.Id;
                    acts.Add(currentAct);
                }
            }
            return acts;
        }

        void RefreshTreeView()
        {
            //clean the treeview
            tvUseCase.Nodes.Clear();
            //create the activity nodes under the usecase
            List<TreeNode> children = new List<TreeNode>();
            UseCaseTobeEdited.Activities.ForEach(act =>
            {
                if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                {
                    TreeNode child = new TreeNode(!string.IsNullOrEmpty(act.Name)? act.Name: "Activity- " + act.Id);
                    child.Tag = act;
                    children.Add(child);
                }
            });
            //create the parent node
            TreeNode parent = new TreeNode("Use Case- " + UseCaseTobeEdited.Name, children.ToArray());
            parent.Tag = UseCaseTobeEdited;
            tvUseCase.Nodes.Add(parent);
            tvUseCase.ExpandAll();
            tvUseCase.AllowDrop = true;
        }

        private void splitContainer1_Panel2_Click(object sender, EventArgs e)
        {
            //set the focus to the panel 2 so that the scrolling using mouse wheel may work
            splitContainer1.Panel2.Focus();
        }
    }
}
