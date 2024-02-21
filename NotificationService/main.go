package main

import (
	"fmt"
	"net/http"
	"os"

	"github.com/gin-gonic/gin"
)

// Need to change this so that it becomes event driven
// it gets message and will be responsible for sending notifications

func main() {
	environmentValidation()
	router := gin.Default()
	router.POST("/notification", postNotification)

	router.Run("localhost:8080")
}

func environmentValidation() {
	accountId := os.Getenv("TWILIO_ACCOUNT_SID")
	authToken := os.Getenv("TWILIO_AUTH_TOKEN")
	//
	if accountId == "" || authToken == "" {
		fmt.Errorf("Expecting twilio environment vars")
		os.Exit(1)
	}

}
func postNotification(c *gin.Context) {
	var event NotificationEvent

	if err := c.BindJSON(&event); err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Expecting notification event in body."})
		return
	}
	// TODO: add validation
	// Type could only be sms or email
	// email and phone number validations

	if event.Type == "email" {
		sendEmail()
	} else {
		errSend := sendSms(event)
		if errSend != nil {
			fmt.Errorf(errSend.Error())
			c.IndentedJSON(http.StatusInternalServerError, gin.H{"error": "Error encountered sending message"})
		}
	}

	c.IndentedJSON(http.StatusCreated, event)
}
