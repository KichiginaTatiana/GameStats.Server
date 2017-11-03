//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Match
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Match()
        {
            this.Scoreboards = new HashSet<Scoreboard>();
        }
    
        public long Id { get; set; }
        public string ServerEndpoint { get; set; }
        public string GameMode { get; set; }
        public string Map { get; set; }
        public long FragLimit { get; set; }
        public long TimeLimit { get; set; }
        public double TimeElapsed { get; set; }
        public string Timestamp { get; set; }
    
        public virtual Server Server { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scoreboard> Scoreboards { get; set; }
    }
}
