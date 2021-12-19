from twilio.rest import Client 
 
account_sid = 'AC' 
auth_token = '[AuthToken]' 
client = Client(account_sid, auth_token) 
 
message = client.messages.create( 
                              from_='whatsapp:+14',  
                              body='Your appointment is coming up on July 21 at 3PM',      
                              to='whatsapp:+9181' 
                          ) 
 
print(message.sid)