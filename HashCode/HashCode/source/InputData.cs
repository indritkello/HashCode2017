using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orc.Sort;

namespace HashCode
{
    public class InputData
    {

        public int[] videoSizes;
        public Endpoint[] endPoints;
        public List<RequestDesc> requestDescriptions = new List<RequestDesc>();
        public int cacheSize;
        public List<Cache> Caches = new List<Cache>();

        public int minVideoSize = Int32.MaxValue;

        public InputData(string filePath)
        {
            int linesRead = 0;
            int currentEndpoint = 0;
            int cachesNumber = 0;
            using (StreamReader str = new StreamReader(filePath))
            {
                while (!str.EndOfStream)
                {

                    string nextLine = str.ReadLine();
                    linesRead++;
                    string[] lineData = nextLine.Split(' ');

                    if (linesRead == 1)
                    {
                        videoSizes = new int[Convert.ToInt32(lineData[0])];
                        endPoints = new Endpoint[Convert.ToInt32(lineData[1])];
                        cachesNumber = Convert.ToInt32(lineData[3]);
                        cacheSize = Convert.ToInt32(lineData[4]);
                        //cacheLatencies = new CacheEndpointLatency[Convert.ToInt32(lineData[3])];
                    }
                    else if (linesRead == 2)
                    {
                        for (int i = 0; i < lineData.Length; i++)
                        {
                            int videoSize = Convert.ToInt32(lineData[i]);
                            videoSizes[i] = videoSize;
                            if (videoSize < minVideoSize)
                            {
                                minVideoSize = videoSize;
                            }
                        }
                    }
                    else if (lineData.Length < 3)
                    {
                        int lRead = linesRead - 2;
                        var endpoint = new Endpoint
                        {
                            DataCenterLatency = Convert.ToInt32(lineData[0]),
                            CacheLatencies = new List<CacheEndpointLatency>(),
                            Id = currentEndpoint
                        };

                        int linkedCaches = Convert.ToInt32(lineData[1]);
                        for (int k = 0; k < linkedCaches; k++)
                        {
                            string[] lData = str.ReadLine().Split(' ');
                            linesRead++;
                            var latency = new CacheEndpointLatency
                            {
                                CacheId = Convert.ToInt32(lData[0]),
                                Value = Convert.ToInt32(lData[1]),
                                EndPointId = currentEndpoint,
                                Difference = endpoint.DataCenterLatency - Convert.ToInt32(lData[1])
                            };
                            endpoint.CacheLatencies.Add(latency);
                        }
                        endPoints[currentEndpoint] = endpoint;
                        currentEndpoint++;
                        //endPoints = endPoints.OrderBy(xx => xx.Id).ToArray();
                    }
                    else
                    {
                        var request = new RequestDesc
                        {
                            VideoId = Convert.ToInt32(lineData[0]),
                            EndPointId = Convert.ToInt32(lineData[1]),
                            RequestNumber = Convert.ToInt32(lineData[2])
                        };
                        requestDescriptions.Add(request);
                        endPoints.First(xx => xx.Id == request.EndPointId).TotalRequests += request.RequestNumber;
                    }

                }
            }
            foreach (var request in requestDescriptions)
            {
                request.Probability = (decimal)request.RequestNumber / endPoints.First(xx => xx.Id == request.EndPointId).TotalRequests;
            }
            for (int i = 0; i < cachesNumber; i++)
            {
                Caches.Add(new Cache
                {
                    CacheId = i,
                    CacheSize = cacheSize
                });
            }

        }


        public void sortRequestDescriptionsByProbability(List<RequestDesc> input)
        {
            TimSortExtender.TimSort(input);
        }
    }
}
