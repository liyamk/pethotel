package main

import "time"

type Token struct {
	Token      string    `json:"token"`
	Expiration time.Time `json:"expiration"`
}

type Reservation struct {
	Id                  int       `json:"id"`
	RoomType            RoomType  `json:"roomType"`
	ExpectedCheckInTime time.Time `json:"expectedCheckInTime"`
	CheckInTime         time.Time `json:"checkInTime"`
	CheckOutTime        time.Time `json:"checkOutTime"`
	PetId               int       `json:"PetId"`
}

type RoomType string

const (
	Standard RoomType = "Standard"
	Deluxe   RoomType = "Deluxe"
)
