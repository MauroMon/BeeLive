using BeeLive.NoiseData.Core.Entities;
using BeeLive.NoiseData.Core.Repositories;
using BeeLive.Persistence;
using BeeLive.Persistence.Repositories;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeLive.NoiseData.Persistence
{
    public class NoiseDataRepository : Repository<Core.Entities.NoiseData>, INoiseDataRepository
    {
        public NoiseDataRepository(ElasticSearchContext context, IOptions<ElasticSearchSettings> settings) : base(context, settings.Value.NoiseDataIndex)
        {
        }

        public async Task<NoiseDataAvg> GetAverage(DateTime dtFrom, DateTime dtTo, int hiveId)
        {
            var result = await Context.Client.SearchAsync<Core.Entities.NoiseData>(sd => sd.Index(IndexName)
                .Aggregations(ag =>
                    ag.Filter("filter", filter => filter.Filter(
                        query =>
                        {
                            var queryContainer = BuildDefaultQuery(dtFrom, dtTo, hiveId);
                            return query.Bool(bq => bq.Must(queryContainer.ToArray()));
                        })
                    .Aggregations(a =>
                        a.Average("average", avg => avg.Field(n => n.Decibel)))))
                );
            //build outpout data
            NoiseDataAvg noiseDataAvg = new NoiseDataAvg();
            var mainBucket = result.Aggregations.Filter("filter");
            noiseDataAvg.Count = mainBucket.DocCount;
            var avg = mainBucket.Average("average").Value;
            noiseDataAvg.Average = avg.HasValue ? Convert.ToDecimal(avg.Value) : 0;
            return noiseDataAvg;
        }

        public async Task<NoiseDataCount> CountAsync(DateTime dtFrom, DateTime dtTo, int hiveId)
        {
            var result = await Context.Client.SearchAsync<Core.Entities.NoiseData>(sd => sd.Index(IndexName)
                .Aggregations(ag =>
                    ag.Filter("filter", filter => filter.Filter(
                        query =>
                        {
                            var queryContainer = BuildDefaultQuery(dtFrom, dtTo, hiveId);
                            return query.Bool(bq => bq.Must(queryContainer.ToArray()));
                        })
                    .Aggregations(a =>
                        a.Filter("WarningFilter", warning =>
                            warning.Filter(f =>
                                f.Term(n =>
                                   n.Warning, true))))))
                );
            //build output data
            NoiseDataCount noiseDataCount = new NoiseDataCount();
            var mainBucket = result.Aggregations.Filter("filter");
            noiseDataCount.Total = mainBucket.DocCount;
            noiseDataCount.Warning = mainBucket.Filter("WarningFilter").DocCount;
            return noiseDataCount;
        }

        private List<QueryContainer> BuildDefaultQuery(DateTime dtFrom, DateTime dtTo, int hiveId)
        {
            var queryContainer = new List<QueryContainer>();
            var HiveIdQuery = Query<Core.Entities.NoiseData>.Term(n => n.HiveId, hiveId);
            var dtFromQuery = Query<Core.Entities.NoiseData>.DateRange(dr => dr.Field(n => n.Dt).GreaterThanOrEquals(dtFrom));
            var dtToQuery = Query<Core.Entities.NoiseData>.DateRange(dr => dr.Field(n => n.Dt).LessThanOrEquals(dtTo));
            queryContainer.Add(HiveIdQuery);
            queryContainer.Add(dtFromQuery);
            queryContainer.Add(dtToQuery);
            return queryContainer;
        }
    }
}
