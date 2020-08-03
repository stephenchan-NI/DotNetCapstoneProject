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
        string resourceName;
        double refLevel;
        double freq;
        double segmentLength;
        double measureOffset;
        int numSeg;



        bool enableTrigger;
        string digitalEdgeSource;
        RFmxSpecAnMXDigitalEdgeTriggerEdge digitalEdge;

        RFmxSpecAnMXPavtMeasurementLocationType measurementLocationType;

        double segment0StartTime;
        double segmentInterval;

        const int segmentStartTimeArraySize = 1;
        double[] segmentStartTime = new double[segmentStartTimeArraySize];

        double timeout;

        double[] meanRelativePhase = new double[0];                          /* (deg) */
        double[] meanRelativeAmplitude = new double[0];                      /* (dB) */
        double[] meanAbsolutePhase = new double[0];                          /* (deg) */
        double[] meanAbsoluteAmplitude = new double[0];                      /* (dBm) */

        public PavtMeasurement(string rfsaName, double refLvl, double frequency, double measLength, double measOffset, int numSegments )
        {
            resourceName = rfsaName;
            refLevel = refLvl;
            freq = frequency;
            segmentLength = measLength;
            measureOffset = measOffset;
            numSeg = numSegments;
        }
    }
}
