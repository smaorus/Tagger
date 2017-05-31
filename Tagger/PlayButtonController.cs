using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagger
{
    class PlayStateController
    {
        private bool IsEnabled { get; set; }
        private bool IsPaused { get; set; }
        public event EventHandler PlayCommand;
        public event EventHandler PauseCommand;
        public event EventHandler EnableCommand;
        public event EventHandler DisableCommand;

        public PlayStateController()
        {
            IsEnabled = false;
            IsPaused = true;
        }

        public void Enable()
        {
            IsEnabled = true;
            EnableCommand?.Invoke(this, EventArgs.Empty);

            IsPaused = false;
            PlayCommand?.Invoke(this, EventArgs.Empty);
        }

        public void Disable()
        {
            IsPaused = true;
            PauseCommand?.Invoke(this, EventArgs.Empty);

            IsEnabled = false;
            DisableCommand?.Invoke(this, EventArgs.Empty);
        }

        public void Switch()
        {
            if (IsEnabled)
            {
                if (IsPaused)
                {
                    IsPaused = false;
                    PlayCommand?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    IsPaused = true;
                    PauseCommand?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
