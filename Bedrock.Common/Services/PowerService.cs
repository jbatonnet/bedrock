using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public abstract class PowerService : Service
    {
        public abstract bool Value { get; set; }

        public event EventHandler<bool> PowerChanged;
        protected void OnPowerChanged(bool power)
        {
            PowerChanged?.Invoke(this, power);
        }

        public virtual void PowerOn()
        {
            Value = true;
            OnPowerChanged(true);
        }
        public virtual void PowerOff()
        {
            Value = false;
            OnPowerChanged(false);
        }
        public virtual void Switch()
        {
            if (Value)
                PowerOff();
            else
                PowerOn();
        }
    }
}