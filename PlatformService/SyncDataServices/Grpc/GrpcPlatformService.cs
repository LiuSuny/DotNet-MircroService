using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepo platformRepo, IMapper mapper)
         {
            _mapper = mapper;
            _platformRepo = platformRepo;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest allRequest, ServerCallContext server)
        {
              //CREATING NEW RESPONSE 
               var response = new PlatformResponse();
               var platform = _platformRepo.GetAllPlatforms();

               foreach(var plat in platform)
               {
                  response.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));
               }

               return Task.FromResult(response);
        }
    }
}