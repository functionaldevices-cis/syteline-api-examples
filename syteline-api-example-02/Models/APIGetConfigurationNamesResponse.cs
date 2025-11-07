using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syteline_api_examples.Models;

public class APIGetConfigurationsResponse : APIMethodResponse
{
    public List<string> Configurations
    {
        get; set;
    }

    public APIGetConfigurationsResponse(List<string>? configurations = null)
    {
        this.Configurations = configurations ?? [];
    }
}
