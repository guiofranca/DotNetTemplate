using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Domain.Models.Shared;

namespace Template.Domain.Models;

public class BlogPost : Model
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public IEnumerable<BlogComment> BlogComments { get; set; }
}
