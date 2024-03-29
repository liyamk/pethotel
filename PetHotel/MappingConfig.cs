﻿using AutoMapper;
using PetHotel.Models.Dto;
using PetHotel.Models;

namespace PetHotel
{
    /// <summary>
    /// This will be use to map properties from one type to other
    /// </summary>
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<PetDto, Pet>().ReverseMap();
            CreateMap<Pet, PetDto>();
            CreateMap<Pet, PetCreateDto>().ReverseMap();

            CreateMap<Reservation, ReservationCreateDto>().ReverseMap();
            CreateMap<Reservation, ReservationDto>();
            CreateMap<Reservation, ReservationUpdateDto>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();   
            CreateMap<User, UserCreateDto>().ReverseMap();

            CreateMap<Owner, OwnerDto>();
            CreateMap<Owner, OwnerCreateDto>().ReverseMap();
        }
    }
}
