package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net/http"
	"os"
)

// https://github.com/jackc/pgx
func main() {
	apiBaseUrl := os.Getenv("API_BASE")
	token := getToken(apiBaseUrl)
	getReservations(apiBaseUrl, token)
	fmt.Println("")
}

func getToken(apiBaseUrl string) Token {
	user := os.Getenv("USER_NAME")
	pass := os.Getenv("PASS")

	loginUrl := apiBaseUrl + "Login"

	postBody, _ := json.Marshal(map[string]string{
		"userName": user,
		"password": pass,
	})

	responseBody := bytes.NewBuffer(postBody)

	resp, err := http.Post(loginUrl, "application/json", responseBody)
	validateResponse(err)

	defer resp.Body.Close()
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		log.Fatalln(err)
	}
	// fmt.Println(string(body))
	var token Token
	json.Unmarshal(body, &token)
	// fmt.Println("Token", token.Token)
	return token
}

func getReservations(apiBaseUrl string, token Token) {
	client := &http.Client{}
	reservationUrl := apiBaseUrl + "Reservations"
	req, _ := http.NewRequest("GET", reservationUrl, nil)
	req.Header.Add("Authorization", "Bearer "+token.Token)

	resp, err := client.Do(req)
	validateResponse(err)
	defer resp.Body.Close()
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		log.Fatalln(err)
	}
	fmt.Println(string(body))
}

func validateResponse(err error) {
	if err != nil {
		fmt.Print("Encountered error", err)
	}
}
