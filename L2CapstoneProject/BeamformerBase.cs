using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CapstoneProject
{
    abstract class BeamformerBase
    {
        public abstract void connect();
        public abstract void configurePhaseAmplitudeOffset(double phase, double amp);
        public abstract void commitPhaseAmplitudeRegister();
        public abstract void abort();
        public abstract void disconnect();
    }
}
