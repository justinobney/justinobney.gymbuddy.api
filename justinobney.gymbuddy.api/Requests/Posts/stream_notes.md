# stream notes

## timeline types

* standard to all
    * userName
    * profilePictureUrl
    * timestamp
    * kudosCount
    * comments
* text
    * text
* image
    * text
    * imageUrl
* workout
    * muscleGroup
    * scheduleDate
    * guests `[ ]`
        * userName
        * profilePictureUrl

## streams

* user_posts (flat)
* user (flat)
* timeline_flat (flat)
* notification (notification)

## activity

* user_posts
    * gets "timeline activities"
* user (flat) (TODO: later)
    * gets notifications via "to"

## follows

`timeline_flat:a` => `user_posts:b`
