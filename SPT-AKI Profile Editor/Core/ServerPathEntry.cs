﻿namespace SPT_AKI_Profile_Editor.Core
{
    public class ServerPathEntry
    {
        public ServerPathEntry(string key, string path, bool isFounded)
        {
            Key = key;
            Path = path;
            IsFounded = isFounded;
        }

        public string Key { get; }
        public string Path { get; set; }
        public bool IsFounded { get; }
    }
}