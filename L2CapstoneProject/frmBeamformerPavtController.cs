﻿using System;
using System.Windows.Forms;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.ModularInstruments.SystemServices.DeviceServices;
using System.Collections.Generic;

namespace L2CapstoneProject
{

    public partial class frmBeamformerPavtController : Form
    {
        NIRfsg rfsg;
        RFmxInstrMX instr;

        public struct PhaseAmplitudeOffset 
        {
            public double phase { get; set; }
            public double amplitude { get; set; }
        }

        public frmBeamformerPavtController()
        {
            InitializeComponent();
            LoadDeviceNames();
        }

        private void LoadDeviceNames()
        {
            ModularInstrumentsSystem rfsgSystem = new ModularInstrumentsSystem("NI-Rfsg");
            foreach (DeviceInfo device in rfsgSystem.DeviceCollection)
                rfsgNameComboBox.Items.Add(device.Name);
            if (rfsgSystem.DeviceCollection.Count > 0)
                rfsgNameComboBox.SelectedIndex = 0;

            ModularInstrumentsSystem rfmxSystem = new ModularInstrumentsSystem("NI-Rfsa");
            foreach (DeviceInfo device in rfmxSystem.DeviceCollection)
                rfsaNameComboBox.Items.Add(device.Name);
            if (rfsgSystem.DeviceCollection.Count > 0)
                rfsaNameComboBox.SelectedIndex = 0;
        }
        #region UI Events
        private void btnAddOffset_Click(object sender, EventArgs e)
        {
            AddOffset();
        }
        private void EditListViewItem(object sender, EventArgs e)
        {
            if (CheckSelection(out int selected))
            {
                EditOffset(selected);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (CheckSelection(out int selected))
            {
                RemoveOffset(selected);
            }
        }
        private void lsvOffsets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckSelection(out int _))
            {
                btnDeleteOffset.Enabled = btnEditOffset.Enabled = true;
            }
            else
            {
                btnDeleteOffset.Enabled = btnEditOffset.Enabled = false;
            }
        }
        private void lsvOffsets_KeyDown(object sender, KeyEventArgs e)
        {
            if (CheckSelection(out int selected))
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        EditOffset(selected);
                        break;
                    case Keys.Delete:
                        RemoveOffset(selected);
                        break;
                }
            }
        }

        private void frmBeamformerPavtController_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseInstruments();
        }

        #endregion
        #region Program Functions
        private void AbortGeneration()
        {
            SetButtonState(false);

            if (rfsg?.IsDisposed == false)
            {
                rfsg.Abort();
            }
        }
        private void CloseInstruments()
        {
            AbortGeneration();
            rfsg?.Close();

            instr?.Close();
        }
        private void SetButtonState(bool started)
        {
            btnStart.Enabled = !started;
            btnStop.Enabled = started;
        }
        void ShowError(string functionName, Exception exception)
        {
            AbortGeneration();
            errorTextBox.Text = "Error in " + functionName + ": " + exception.Message;
        }
        void SetStatus(string statusMessage)
        {
            errorTextBox.Text = statusMessage;
        }
        #endregion
        #region Offset Functions
        private void AddOffset()
        {
            frmOffset dialog = new frmOffset(frmOffset.Mode.Add);
            DialogResult r = dialog.ShowDialog();

            if (r == DialogResult.OK)
            {
                // Add the offset to the listview
                lsvOffsets.Items.Add(dialog.phaseAmpOffset.phase.ToString()).SubItems.Add(dialog.phaseAmpOffset.amplitude.ToString());
                
            }
        }
        private void EditOffset(int selected)
        {
            // Will need to pass in the currently selected item
            frmOffset dialog = new frmOffset(frmOffset.Mode.Edit);

            DialogResult r = dialog.ShowDialog();

            if (r == DialogResult.OK)
            {
                // Edit the offset shown in the listview
                lsvOffsets.Items.RemoveAt(selected);
                lsvOffsets.Items.Insert(selected, dialog.phaseAmpOffset.phase.ToString()).SubItems.Add(dialog.phaseAmpOffset.amplitude.ToString());
            }
        }

        ///
        private void RemoveOffset(int selected)
        {
            lsvOffsets.Items.RemoveAt(selected);
        }
        #endregion
        #region Utility Functions

        /// <summary>
        /// Validates that the listview has at least one value selected. Optionally returns the selected index.
        /// </summary>
        /// <param name="selectedIndex">Current selected index in the list view.</param>
        /// <returns></returns>
        private bool CheckSelection(out int selectedIndex)
        {
            if (lsvOffsets.SelectedItems.Count == 1)
            {
                selectedIndex = lsvOffsets.SelectedIndices[0];
                return true;
            }
            else
            {
                selectedIndex = -1;
                return false;
            }
        }

        #endregion

        private void btnStart_Click(object sender, EventArgs e)
        {
            List<PhaseAmplitudeOffset> offsetTable = new List<PhaseAmplitudeOffset>();
          
            foreach (ListViewItem item in lsvOffsets.Items)
            {
                PhaseAmplitudeOffset tempSet = new PhaseAmplitudeOffset();
                tempSet.phase = Convert.ToDouble(item.Text);
                tempSet.amplitude = Convert.ToDouble(item.SubItems[1].Text);

                offsetTable.Add(tempSet);
            }

            RfsgSequencedBeamformer seqBeam = new RfsgSequencedBeamformer(decimal.ToDouble(measurementLengthNumeric.Value), rfsgNameComboBox.SelectedItem.ToString());
            seqBeam.connect();
            seqBeam.downloadPhaseAmplitudeOffset(offsetTable);
        }
    }
}