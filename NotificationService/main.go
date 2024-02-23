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

	emailAuth := os.Getenv("EMAIL_AUTH_PASSWORD")
	if emailAuth == "" {
		fmt.Errorf("Expecting email auth password environment var")
	}

}

func postNotification(c *gin.Context) {
	var event NotificationEvent

	if err := c.BindJSON(&event); err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Expecting notification event in body."})
		return
	}

	if event.Message == "" || (event.Type != "Email" && event.Type != "Sms") {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Invalid request, message empty or type not set to either 'Email' or 'Sms'"})
		return
	}

	if event.Type == "email" {
		sendEmail(event, c)
	} else {
		sendSms(event, c)
	}
}
