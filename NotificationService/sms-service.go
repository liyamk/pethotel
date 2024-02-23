package main

import (
	"fmt"
	"net/http"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/twilio/twilio-go"
	api "github.com/twilio/twilio-go/rest/api/v2010"
)

// https://www.twilio.com/docs/messaging/tutorials/automate-testing
func sendSms(event NotificationEvent, c *gin.Context) {
	// Find your Account SID and Auth Token at twilio.com/console
	// and set the environment variables. See http://twil.io/secure
	client := twilio.NewRestClient()

	params := &api.CreateMessageParams{}
	params.SetBody(event.Message)

	from := os.Getenv("FROM_PHONE_NUMBER")
	if from == "" {
		from = "+15005550006" // default to magic number for testing
	}

	params.SetFrom(from)
	params.SetTo(event.PhoneNumber)

	resp, err := client.Api.CreateMessage(params)
	if err == nil {
		if resp.Sid != nil {
			fmt.Println(*resp.Sid)
		} else {
			fmt.Println(resp.Sid)
		}

		c.IndentedJSON(http.StatusCreated, "Notification sent to sms successfully.")
		return
	}

	c.IndentedJSON(http.StatusInternalServerError, gin.H{"error": "Error encountered sending message"})
}
