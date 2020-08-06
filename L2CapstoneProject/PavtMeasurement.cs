using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.RFmx.InstrMX;

namespace L2CapstoneProject
{
    class PavtMeasurement
    {
        RFmxInstrMX instrSession = null;
        RFmxSpecAnMX specAn;

        string resourceName;
        double refLevel;
        double freq;
        double segmentLength;
        double measureOffset;
        double externalAttenuation = 0;
        //double measurementBandwidth = 1e6;
        int numSeg;


        //double measurementLength;


        bool enableTrigger = true;
        string digitalEdgeSource = RFmxSpecAnMXConstants.PxiTriggerLine0;
        RFmxSpecAnMXDigitalEdgeTriggerEdge digitalEdge = RFmxSpecAnMXDigitalEdgeTriggerEdge.Rising;

        RFmxSpecAnMXPavtMeasurementLocationType measurementLocationType = RFmxSpecAnMXPavtMeasurementLocationType.Time;


        const int segmentStartTimeArraySize = 1;
        double[] segmentStartTime;// = new double[numSeg];


        double[] meanRelativePhase = new double[0];                          /* (deg) */
        double[] meanRelativeAmplitude = new double[0];                      /* (dB) */
        double[] meanAbsolutePhase = new double[0];                          /* (deg) */
        double[] meanAbsoluteAmplitude = new double[0];                      /* (dBm) */

        public PavtMeasurement(string rfsaName, double refLvl, double frequency, double measLength, double measOffset, int numSegments)
        {
            resourceName = rfsaName;
            refLevel = refLvl;
            freq = frequency;
            segmentLength = measLength/1e6;
            measureOffset = measOffset/1e6;
            numSeg = numSegments;
            
        }

        
        private void InitializeInstr()
        {
            /* Create a new RFmx Session */
            if (instrSession == null)
            {
                instrSession = new RFmxInstrMX(resourceName, "");
            }
            
        }

        private void ConfigureSpecAn()
        {
            segmentStartTime = new double[numSeg];
            for (int i = 0; i < numSeg; i++)
            {
                segmentStartTime[i] = segmentLength * i;
            }

            //measurementLength = numSeg * segmentLength;

            /* Get SpecAn signal */
            specAn = instrSession.GetSpecAnSignalConfiguration();

            /* Configure measurement */
            specAn.ConfigureRF("", freq, refLevel, externalAttenuation);
            specAn.ConfigureDigitalEdgeTrigger("", digitalEdgeSource, digitalEdge, 0, enableTrigger);
            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Pavt, true);
            specAn.Pavt.Configuration.ConfigureMeasurementLocationType("", measurementLocationType);
            specAn.Pavt.Configuration.ConfigureNumberOfSegments("", numSeg);


            specAn.Pavt.Configuration.ConfigureMeasurementIntervalMode("", RFmxSpecAnMXPavtMeasurementIntervalMode.Uniform);
            
            specAn.Pavt.Configuration.ConfigureSegmentStartTimeList("", segmentStartTime);
            
            specAn.Pavt.Configuration.ConfigureMeasurementInterval("", measureOffset, segmentLength);

            

        }

        public void connectRFmx()
        {
            InitializeInstr();
            ConfigureSpecAn();
        }
        public void initiateMeasure()
        {
            specAn.Initiate("", "");
        }
        public frmBeamformerPavtController.PhaseAmplitudeOffset[] FetchResults()
        {
            specAn.Pavt.Results.FetchPhaseAndAmplitudeArray("", 10, ref meanRelativePhase,
                  ref meanRelativeAmplitude, ref meanAbsolutePhase, ref meanAbsoluteAmplitude);

            frmBeamformerPavtController.PhaseAmplitudeOffset[] resultsArray = new frmBeamformerPavtController.PhaseAmplitudeOffset[meanAbsoluteAmplitude.Length];
            int i = 0;
            
            foreach (double amp in meanRelativeAmplitude)
            {
                resultsArray[i].amplitude = amp;
                resultsArray[i].phase = meanRelativePhase[i];
                i++;
            }
            return resultsArray;
        }

        public void Close()
        {
            instrSession?.Close();
        }
    }
}
