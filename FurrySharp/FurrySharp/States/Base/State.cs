﻿namespace FurrySharp.States
{
    public class State
    {
        public bool Exit { get; set; } = false;
        public bool UpdateEntities { get; protected set; } = true;
        public bool DrawPlayState { get; protected set; } = true;

        public virtual void Create()
        {
        }

        public virtual void Initialize()
        {

        }

        public virtual void UpdateState()
        {

        }

        public virtual void DrawState()
        {
        }

        public virtual void DrawUI()
        {
        }
    }
}