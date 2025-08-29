using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbFirst.Dots;
using DbFirst.Models;

namespace DbFirst.Profiles
{
    public class TodoListProfile : Profile
    {
        public TodoListProfile()
        {
            CreateMap<TodoList, TodoListSelectDto>()
                .ForMember(
                    dest => dest.InsertEmployeeName,
                    opt => opt.MapFrom(src => src.insertEmployeeName + "(" + src.todoId + ")")
                )
                .ForMember(
                    dest => dest.UpdateEmployeeName,
                    opt => opt.MapFrom(src => src.updateEmployeeName + "(" + src.todoId + ")")
                );
        }
    }
}
