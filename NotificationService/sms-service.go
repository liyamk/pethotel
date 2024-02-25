package main

import (
	"fmt"
	"net/http"
	"os"
	"regexp"
	"strings"

	"github.com/gin-gonic/gin"
	"github.com/twilio/twilio-go"
	api "github.com/twilio/twilio-go/rest/api/v2010"
)

// https://www.twilio.com/docs/messaging/tutorials/automate-testing
func sendSms(event NotificationEvent, c *gin.Context) {
	// Find your Account SID and Auth Token at twilio.com/console
	// and set the environment variables. See http://twil.io/secure
	phoneNumber := strings.ReplaceAll(event.PhoneNumber, " ", "")
	phoneNumber = strings.ReplaceAll(phoneNumber, "-", "")
	isValid := isValidNumber(phoneNumber)
	if !isValid {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Invalid phone number " + event.PhoneNumber})
		return
	}

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

func isValidNumber(phoneNumber string) bool {
	if phoneNumber == "" {
		return false
	}

	usNumberRegex := `^\+1\d{10}$` // matches us 10 digit number with +1
	re := regexp.MustCompile(usNumberRegex)

	return re.Find([]byte(phoneNumber)) != nil
}
