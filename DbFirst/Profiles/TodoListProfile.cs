using AutoMapper;
using DbFirst.Dtos;
using DbFirst.Models;

namespace DbFirst.Profiles
{
    public class TodoListProfile : Profile
    {
        public TodoListProfile()
        {
            CreateMap<TodoList, TodoListDto>()
            .ForMember(
            dest => dest.InsertEmployeeName,
            opt => opt.MapFrom(src => src.InsertEmployee.Name + "(" + src.InsertEmployeeId + ")")
            )
            .ForMember(
            dest => dest.UpdateEmployeeName,
            opt => opt.MapFrom(src => src.UpdateEmployee.Name + "(" + src.UpdateEmployeeId + ")")
            );
        }
    }
}
