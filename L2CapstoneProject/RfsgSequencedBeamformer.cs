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
        private double sampleRate = 10e6;
        public double segmentLength { get; set; }
        ComplexWaveform<ComplexDouble> sequenceWfm = new ComplexWaveform<ComplexDouble>(0);

        NIRfsg rfsgSession;
        string resourceName,  script;
        double centerFrequency, powerLevel;
        IntPtr rfsgHandle;
        public RfsgSequencedBeamformer(double measLength, string rfsgName, double power, double freq)
        {
            segmentLength = measLength;
            resourceName = rfsgName;
            centerFrequency = freq;
            powerLevel = power;

        }
        public override void abort()
        {
            throw new NotImplementedException();
        }

        public override void commitPhaseAmplitudeRegister()
        {

            rfsgSession.Arb.WriteWaveform("waveform", sequenceWfm);
            script = @"script GenerateWfmWithMarkerAtStart
                     repeat forever
                     generate waveform marker0(0)
                  end repeat
               end script";
            rfsgSession.Arb.Scripting.WriteScript(script);

        }
        public override void configurePhaseAmplitudeOffset(double phase, double amp)
        {
            phase = phase * (Math.PI / 180);
            
            double numSamples = sampleRate * segmentLength/1000000;
            double real = Math.Cos(phase) * Math.Pow(10, (amp - 10) / 20);
            double imag = Math.Sin(phase) * Math.Pow(10, (amp - 10) / 20);
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

        }

        public override void connect()
        {
            InitializeRfsg();
            rfsgSession.Initiate();
        }

        public override void disconnect()
        {
            rfsgSession.Abort();
            rfsgSession.Close();
            sequenceWfm = new ComplexWaveform<ComplexDouble>(0);
        }
        void InitializeRfsg()
        {
            rfsgSession = new NIRfsg(resourceName, true, false, "");
            rfsgHandle = rfsgSession.GetInstrumentHandle().DangerousGetHandle();
            rfsgSession.Arb.IQRate = sampleRate;
            rfsgSession.RF.PowerLevelType = RfsgRFPowerLevelType.PeakPower;
            rfsgSession.Arb.GenerationMode = RfsgWaveformGenerationMode.Script;
            rfsgSession.DeviceEvents.MarkerEvents[0].ExportedOutputTerminal = RfsgMarkerEventExportedOutputTerminal.PxiTriggerLine0;
            rfsgSession.RF.Configure(centerFrequency, powerLevel);
            commitPhaseAmplitudeRegister();
        }
        public void close()
        {
            rfsgSession?.Abort();
            rfsgSession?.Close();
        }
    }
}
