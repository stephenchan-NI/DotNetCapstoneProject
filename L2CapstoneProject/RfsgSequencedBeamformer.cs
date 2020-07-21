using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;

namespace L2CapstoneProject
{
    class RfsgSequencedBeamformer : BeamformerBase
    {
        private double sampleRate = 100e6;
        public double segmentLength { get; set; }
        ComplexWaveform<ComplexDouble> sequenceWfm;

        NIRfsg rfsgSession;
        string resourceName, waveformName, script, markerEventExportOutputTerminal;
        double centerFrequency, powerLevel, externalAttenuation;
        IntPtr rfsgHandle;
        RfsgSequencedBeamformer(double measLength, string rfsgName)
        {
            segmentLength = measLength;
            resourceName = rfsgName;

        }
        public override void abort()
        {
            throw new NotImplementedException();
        }

        public override void commitPhaseAmplitudeRegister()
        {
            throw new NotImplementedException();
        }
        public override void configurePhaseAmplitudeOffset(double phase, double amp)
        {
            double numSamples = sampleRate * segmentLength;
            double real = Math.Cos(phase) * Math.Pow(amp / 10,10);
            double imag = Math.Sin(phase) * Math.Pow(amp / 10, 10);
            ComplexDouble phaseMag = new ComplexDouble(real, imag);
            ComplexDouble[] wfmArray = Enumerable.Repeat(phaseMag, Convert.ToInt32(numSamples)).ToArray();
            ComplexWaveform<ComplexDouble> wfm = ComplexWaveform<ComplexDouble>.FromArray1D(wfmArray);
            sequenceWfm.Append(wfm);
        }
        public void downloadPhaseAmplitudeOffset(List<frmBeamformerPavtController.PhaseAmplitudeOffset> sequenceList)
        {
            foreach (frmBeamformerPavtController.PhaseAmplitudeOffset phaseAmpOffset in sequenceList)
            {
                double amp = phaseAmpOffset.amplitude;
                double phase = phaseAmpOffset.phase;
                configurePhaseAmplitudeOffset(phase, amp);
            }
            throw new NotImplementedException();
        }

        public override void connect()
        {
            InitializeRfsg();
        }

        public override void disconnect()
        {
            throw new NotImplementedException();
        }
        void InitializeRfsg()
        {
            rfsgSession = new NIRfsg(resourceName, true, false, "");
            rfsgHandle = rfsgSession.GetInstrumentHandle().DangerousGetHandle();
        }
    }
}
