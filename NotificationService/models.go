package main

type NotificationEvent struct {
	Type         string `json:"type"`
	Message      string `json:"message"`
	EmailAddress string `json:"emailAddress"`
	PhoneNumber  string `json:"phoneNumber"`
}
