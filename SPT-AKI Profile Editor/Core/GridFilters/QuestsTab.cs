﻿using System.Collections.Generic;

namespace SPT_AKI_Profile_Editor.Core
{
    public class QuestsTab
    {
        public string TraderNameFilter { get; set; }
        public string QuestNameFilter { get; set; }
        public string QuestStatusFilter { get; set; }
        public Dictionary<string, bool> QuestTypesExpander { get; set; } = new();
    }
}