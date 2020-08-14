using System;
using System.Windows.Forms;
using L2CapstoneProject;

namespace L2CapstoneProject
{
    public partial class frmOffset : Form
    {

        public frmBeamformerPavtController.PhaseAmplitudeOffset phaseAmpOffset = new frmBeamformerPavtController.PhaseAmplitudeOffset();

        public enum Mode { Add, Edit }

        public Mode ViewMode { get; }

        public frmOffset(Mode mode)
        {
            InitializeComponent();
            ViewMode = mode;

            switch (ViewMode)
            {
                case Mode.Add:
                    this.Text = "Add New Offset";
                    break;
                case Mode.Edit:
                    this.Text = "Edit Offset";
                    break;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
            
            phaseAmpOffset.amplitude = decimal.ToDouble(numAmp.Value);
            phaseAmpOffset.phase = decimal.ToDouble(numPhase.Value);
            

        }
    }
}
