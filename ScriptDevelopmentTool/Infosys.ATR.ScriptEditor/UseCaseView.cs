using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.UIAutomation.Entities;

namespace Infosys.ATR.ScriptEditor
{
    public partial class UseCaseView : UserControl
    {
        //public variable
        public class UseCaseChangedArgs: EventArgs
        {
            public UseCase ChangedUseCase { get; set; }
        }
        public delegate void UseCaseChangedEventHandler(UseCaseChangedArgs e);
        public event UseCaseChangedEventHandler UseCaseChanged;

        //private variable
        UseCase _usecase = new UseCase();

        public UseCaseView()
        {
            InitializeComponent();
        }

        public UseCaseView(UseCase useCase)
        {
            InitializeComponent();
            _usecase = useCase;
            txtName.Text = useCase.Name ?? "";
            txtCreatedBy.Text = useCase.CreatedBy ?? "";
            txtCreatedOn.Text = useCase.CreatedOn.ToString();
            txtDesc.Text = useCase.Description ?? "";
            txtDomain.Text = useCase.Domain ?? "";
            txtIP.Text = useCase.MachineIP ?? "";
            txtMachineName.Text = useCase.MachineName ?? "";
            txtMachineType.Text = useCase.MachineType ?? "";
            txtOS.Text = useCase.OS ?? "";

            if (string.IsNullOrWhiteSpace(useCase.Tags))
                useCase.Tags = "";
            string[] tags = useCase.Tags.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            TagItem tagsCotrol = new TagItem(tags);
            tagsCotrol.TagsChanged += new TagItem.TagsChangedEventHandler(tagsCotrol_TagsChanged);
            tagsCotrol.Dock = DockStyle.Fill;
            pnlTags.Controls.Add(tagsCotrol);
        }

        void tagsCotrol_TagsChanged(TagItem.TagsChangedArgs e)
        {
            _usecase.Tags = e.ChangedTags;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //raise an event with the updated use case data
            if (UseCaseChanged != null)
                UseCaseChanged(new UseCaseChangedArgs() { ChangedUseCase = _usecase });
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _usecase.Name = txtName.Text;
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            _usecase.Description = txtDesc.Text;
        }

        private void txtCreatedBy_TextChanged(object sender, EventArgs e)
        {
            _usecase.CreatedBy = txtCreatedBy.Text;
        }

        private void txtCreatedOn_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMachineName_TextChanged(object sender, EventArgs e)
        {
            _usecase.MachineName = txtMachineName.Text;
        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {
            _usecase.Id = txtIP.Text;
        }

        private void txtOS_TextChanged(object sender, EventArgs e)
        {
            _usecase.OS = txtOS.Text;
        }

        private void txtDomain_TextChanged(object sender, EventArgs e)
        {
            _usecase.Domain = txtDomain.Text;
        }

        private void txtMachineType_TextChanged(object sender, EventArgs e)
        {
            _usecase.MachineType = txtMachineType.Text;
        }

        private void UseCaseView_Load(object sender, EventArgs e)
        {
            this.Paint += new PaintEventHandler(UseCaseView_Paint);
        }

        void UseCaseView_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new System.Drawing.Pen(Color.Gray);
            //vertical line for use case details
            int x = txtName.Location.X + txtName.Width + 25;
            int y = txtName.Location.Y;
            e.Graphics.DrawLine(pen, x, y, x, y + 148);

            //horizontal line after the use case details
            x = lblTags.Location.X;
            y = lblTags.Location.Y - 10;
            e.Graphics.DrawLine(pen, x, y, x + 640, y);

            //horizontal line after the tags
            x = lblEnd.Location.X;
            y = lblEnd.Location.Y - 10;
            e.Graphics.DrawLine(pen, x, y, x + 640, y);
        }
    }
}
