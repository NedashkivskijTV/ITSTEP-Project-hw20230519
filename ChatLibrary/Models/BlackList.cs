using System;
using System.Collections.Generic;

namespace ChatLibrary
{

    public partial class BlackList
    {
        public int Id { get; set; }
    
        public int CreatorId { get; set; }
    
        public int BlackUserId { get; set; }
    
        public virtual User BlackUser { get; set; } = null!;
    
        public virtual User Creator { get; set; } = null!;

    }

}