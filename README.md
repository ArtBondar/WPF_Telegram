# Project Overview
I worked on this project as part of a team of three people. Specifically, I was responsible for developing the client application using WPF. We also had a web version and, of course, a server. The essence of our task was to write our own version of Telegram, where users could send messages, photos, create groups and channels where they could communicate. I couldn't find any visual components for this type of messenger, so I decided to post it on Git. I hope I can help someone.

# The Application
## Login
{Login screen}
When you first launch our application, you are greeted with a login window where you need to enter the login and password for your account. There are also a couple of additional buttons such as "Registration" and "Forgot password?" If you click on the registration button, you will be given the opportunity to enter your details, and then a confirmation code will be sent to your email. If you click on the "Forgot password?" button, you will be given the opportunity to enter your email address to reset your password, and a confirmation code will be sent to your email as well. There is also an option to automatically save the user's login and password on their computer. If the login is successful, the data is saved on their computer. The password in this case is stored in encrypted form, and user data is deleted when they log out.

## Main Window
{Main screen}
If we successfully log in, we are greeted with chats. You can open the interaction window by clicking on the button on the left. Here, there are various buttons such as create a group or channel, go to saved messages, go to settings, and change the theme (to dark or light).

## Settings
{Settings screen}
Here, you can configure your account by changing your password, login, or account description. You can also change the theme and the background.

## Chats
There are three types of chats:
1. Private (where one user writes to another)
2. Group (where users can write to each other)
3. Channel (where only the user who created it can write)

## Contacts
{Contacts screen}

## Search
There is a search for channels, groups, and people in the application.

## Creating a Channel or Group
{Creating a Channel or Group screen}
We need to select a photo, name, and description. After that, we need to select the contacts we want to invite to the group or channel

## Message Management
{Message Management screen}
You can also delete your messages or copy the text inside them. You can send photos as well.

# Links
- [WpfAnimatedGif](https://github.com/XamlAnimatedGif/WpfAnimatedGif)
- [emoji.wpf](https://github.com/samhocevar/emoji.wpf)
- [Server](https://github.com/Jumedoo/server)
