using SFML.Graphics;
using System;
using System.Diagnostics;

namespace Shared.Core
{
    public class ApplicationInstanceBase
    {
        public ApplicationInstanceBase()
        {
            Id = Guid.NewGuid();
        }

        public virtual void Stop()
        {
            Debug.WriteLine($"Stop: {GetType().Name}");
            this.IsActive = false;
        }

        public virtual void Start()
        {
            Debug.WriteLine($"Start: {GetType().Name}");
            this.IsActive = true;
        }

        public virtual void Initialize()
        {
            Debug.WriteLine($"Initialize: {GetType().Name}");
            this.IsInitialized = true;
        }

        public virtual void Suspend()
        {
            Debug.WriteLine($"Suspend: {GetType().Name}");
            this.IsActive = false;
        }

        public virtual bool OnException()
        {
            Debug.WriteLine($"Exception thrown and unhandled by: {GetType().Name}");
            return false;
        }
        public Guid Id { get; set; }

        public virtual Screen Screen { get; set; }

        public bool IsInitialized { get; internal set; }

        public bool IsActive { get; internal set; }
    }
}
