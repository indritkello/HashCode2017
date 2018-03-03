using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// @team Pragmatic
/// </summary>
namespace HashCode
{
    class Program
    {
        public static List<Result> Results = new List<Result>();

        public static IEnumerable<object> Resu { get; private set; }

        static void Main(string[] args)
        {
            InputData data = new InputData("me_at_the_zoo.txt");
            
            foreach (var cache in data.Caches)
            {
                Results.Add(new Result {
                    CacheId = cache.CacheId,
                    VideoIds = new List<int>()
                });
            }
            while (data.requestDescriptions.Count > 0 && AreFreeChaches(data.Caches, data.minVideoSize))
            {
                //sort endPoints by data center latency and then by total request (dsc)
                foreach (var endPoint in data.endPoints.OrderByDescending(xx => xx.DataCenterLatency)
                                                        .ThenByDescending(xx => xx.TotalRequests))
                {
                    List<RequestDesc> endPointRequests = data.requestDescriptions.Where(xx => xx.EndPointId == endPoint.Id)
                                                                                 .ToList();
                    //compare probabilities
                    TimSortExtender.TimSort(endPointRequests, CompareRequests);
                    if (endPointRequests.Count <= 0)
                        continue;
                    int videoWithHighestProb = data.videoSizes[endPointRequests.Last().VideoId];
                    List<CacheEndpointLatency> endpointLatencies = endPoint.CacheLatencies;
                    TimSortExtender.TimSort(endpointLatencies, CompareLatencys);
                    
                    foreach (var cacheLatency in endpointLatencies)
                    {
                        Cache toAddVideo = data.Caches.First(xx => xx.CacheId == cacheLatency.CacheId);

                        if (toAddVideo.CacheSize > videoWithHighestProb)
                        {
                            Result toModify = Results.First(xx => xx.CacheId == cacheLatency.CacheId);
                            if (!toModify.VideoIds.Contains(endPointRequests.Last().VideoId))
                            {
                                Results.First(xx => xx.CacheId == cacheLatency.CacheId).VideoIds
                                       .Add(endPointRequests.Last().VideoId);
                            }
                            toAddVideo.CacheSize -= videoWithHighestProb;
                            break;
                        }
                    }
                    data.requestDescriptions.Remove(endPointRequests.Last());

                }
            }
            using (StreamWriter sw = new StreamWriter("output.txt"))
            {
                sw.WriteLine(Results.Count);
                //write video ids for each cache
                foreach (var result in Results)
                {
                    string value = result.CacheId.ToString();
                    foreach (int id in result.VideoIds)
                    {
                        value += " " + id.ToString();
                    }
                    sw.WriteLine(value);
                }
                sw.Flush();
                sw.Close();
            }

        }

        public static int CompareRequests(RequestDesc first, RequestDesc second)
        {
            if (first.Probability > second.Probability)
            {
                return 1;
            }
            else if (first.Probability == second.Probability)
            {
                return 0;
            }
            else {
                return -1;
            }
        }

        public static int CompareLatencys(CacheEndpointLatency first, CacheEndpointLatency second)
        {
            if (first.Value > second.Value)
            {
                return 1;
            }
            else if (first.Value == second.Value)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public static bool AreFreeChaches(List<Cache> input, int minVideoSize)
        {
            return input.Any(xx => xx.CacheSize > minVideoSize);//in order to be capable of having the video
        }
    }
}
