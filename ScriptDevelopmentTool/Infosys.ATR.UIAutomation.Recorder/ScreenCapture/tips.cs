using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Teboscreen
    {
    public partial class tips : Form
        {

        private Form m_InstanceRef = null;
        public Form InstanceRef
            {
            get
                {
                return m_InstanceRef;
                }
            set
                {
                m_InstanceRef = value;
                }
            }

        public tips(string[] Parms)
            {
            InitializeComponent();
            lblTitle.Text = Parms[0];
            }

        private void button3_Click(object sender, EventArgs e)
            {
            this.Close();
            }

        private void tips_Load(object sender, EventArgs e)
            {

            }
        }
    }