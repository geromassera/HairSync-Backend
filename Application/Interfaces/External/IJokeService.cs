using Application.Models.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.External
{
    public interface IJokeService
    {
        Task<JokeDto> GetRandomJokeAsync();
    }
}
