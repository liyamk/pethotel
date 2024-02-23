package main

type NotificationEvent struct {
	Type          string `json:"type"`
	Message       string `json:"message"`
	EmailAddresss string `json:"emailAddresss"`
	PhoneNumber   string `json:"phoneNumber"`
}
