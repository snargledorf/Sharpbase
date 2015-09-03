using System;

namespace Sharpbase
{
    public abstract class FirebaseEventArgs : EventArgs
    {
        protected FirebaseEventArgs(Firebase firebase, Snapshot snapshot)
        {
            Firebase = firebase;
            Snapshot = snapshot;
        }

        public Snapshot Snapshot { get; }

        public Firebase Firebase { get; }
    }
}