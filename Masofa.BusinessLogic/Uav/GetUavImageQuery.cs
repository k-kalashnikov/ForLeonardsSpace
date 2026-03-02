using MediatR;

namespace Masofa.BusinessLogic.Uav.Queries
{
    public class UavImageFileDto
    {
        public Stream FileStream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

    public class GetUavImageQuery : IRequest<UavImageFileDto>
    {
        public Guid PhotoId { get; set; }
    }
}