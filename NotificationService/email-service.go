package main

import (
	"fmt"
	"net/http"
	"net/mail"
	"net/smtp"
	"os"

	"github.com/gin-gonic/gin"
)

func sendEmail(event NotificationEvent, c *gin.Context) {
	if event.EmailAddress == "" {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Invalid email address, empty email address"})
		return
	}

	_, err := mail.ParseAddress(event.EmailAddress)
	if err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{"message": "Invalid email address " + err.Error()})
		return
	}

	from := os.Getenv("FROM_EMAIL_ADDR")
	if from == "" {
		from = "no-reply@pethotel.com"
	}

	to := []string{
		event.EmailAddress,
	}

	smtpHost := "smtp.gmail.com"
	smtpPort := "587"
	message := []byte(event.Message)
	password := os.Getenv("EMAIL_AUTH_PASSWORD")

	auth := smtp.PlainAuth("", from, password, smtpHost)
	errSend := smtp.SendMail(smtpHost+":"+smtpPort, auth, from, to, message)
	if errSend != nil {
		fmt.Println(errSend)
		c.IndentedJSON(http.StatusInternalServerError, gin.H{"error": "Error encountered sending message"})
		return
	}

	c.IndentedJSON(http.StatusCreated, "Notification sent to email successfully.")
}
