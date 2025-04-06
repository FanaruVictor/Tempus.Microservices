using AutoMapper;

namespace Tempus.Shared.Commons;

public static class GenericMapper<TSource, TResult>
    where TSource : class
    where TResult : class
{
    public static TResult Map(TSource source)
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TResult>());
        var mapper = new Mapper(config);

        return mapper.Map<TResult>(source);
    }
}