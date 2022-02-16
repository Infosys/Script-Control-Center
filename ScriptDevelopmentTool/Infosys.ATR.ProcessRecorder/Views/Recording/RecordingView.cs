using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.ATR.ProcessRecorder.Entities;

namespace Infosys.ATR.ProcessRecorder.Views
{
    public partial class RecordingView : UserControl,IClose, IRecording
    {
        public List<UseCasePE> UseCases { get; set; }       

        private bool ascending = false;

        public RecordingView()
        {
            InitializeComponent();
        }

        internal void PopulateView()
        {
            this._presenter.GetUseCases();
            if (UseCases != null)
            {
                this.dgUseCases.DataSource = UseCases;
                DataGridViewColumn column = this.dgUseCases.Columns[0];                
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;                
                this._presenter.UpdateStatus(UseCases.Count());
            }
            else
                this._presenter.UpdateStatus(0);
        }     

        public bool Close()
        {
            this._presenter.OnCloseView();            
            return true;
        }

        private void dgUseCases_SelectionChanged(object sender, EventArgs e)
        {
            if (dgUseCases.SelectedRows != null && dgUseCases.SelectedRows.Count > 0)
            {
                var selected = dgUseCases.SelectedRows[0];
                var selectedTransaction = selected.DataBoundItem as UseCasePE;

                this._presenter.UseCasesFilePath = selectedTransaction.FilePath;

                if (propGrdUsecase.InvokeRequired)
                {
                    propGrdUsecase.Invoke(new Action(() =>
                    {
                        propGrdUsecase.SelectedObjects = new object[] 
                        { 
                           selectedTransaction as UseCasePE
                        };
                        this.propGrdUsecase.Dock = DockStyle.Fill;
                        propGrdUsecase.BrowsableAttributes = new AttributeCollection(new Attribute[] { new PropertyGridBrowsableAttribute(true) });
                    }));
                }
                else
                {
                    propGrdUsecase.SelectedObjects = new object[] { selectedTransaction as UseCasePE };
                    this.propGrdUsecase.Dock = DockStyle.Fill;
                    propGrdUsecase.BrowsableAttributes = new AttributeCollection(new Attribute[] { new PropertyGridBrowsableAttribute(true) });
                }
            }
        }

        private void dgUseCases_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string orderByKey = dgUseCases.Columns[e.ColumnIndex].Name;

            if (ascending)
            {
                dgUseCases.DataSource = UseCases.OrderBy(x => x.GetType().GetProperty(orderByKey).GetValue(x, null)).ToList<UseCasePE>();
                ascending = false;
            }
            else
            {
                dgUseCases.DataSource = UseCases.OrderByDescending(x => x.GetType().GetProperty(orderByKey).GetValue(x, null)).ToList<UseCasePE>();
                ascending = true;
            }
        }

        public void btnGenPlaybackScript_Click(object sender, EventArgs e)
        {
            this._presenter.GeneratePlaybackScript(); 
        }

        public void RunPlaybackScript_Click(object sender, EventArgs e)  
        {
            this._presenter.RunReplaybackScript();
        }       
        //private void dgUseCases_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    DataGridViewCell clickedCell;
        //    try
        //    {
        //        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        //        {
        //            if (e.ColumnIndex == 0)
        //            {
        //                var selected = dgUseCases.SelectedRows[0];
        //                var selectedTransaction = selected.DataBoundItem as UseCasePE;
        //                this._presenter.UseCasesFilePath = selectedTransaction.FilePath;
        //                this._presenter.ReplayScript();
        //            }
        //        }
        //        DataGridView view = (DataGridView)sender;
        //        clickedCell = view.Rows[e.RowIndex].Cells[e.ColumnIndex];
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Error While Executing Playback Script", " Error - Playback Script ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        //if(System.IO.Directory.Exists("20160425121033"))
        //        //System.IO.Directory.Delete("20160425121033",true);
        //    }
        //}
    }
}