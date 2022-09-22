
pipeline
{
    agent any
    stages{
        stage("Git Clone")
        {
            steps
            {
                echo "========Cloning Git========"
            }
            post
            {
                success
                {
                    echo "========Cloning Git successfully========"
                }
                failure
                {
                    echo "========Cloning Git failed========"
                }
            }
        }

        stage('Docker Compose')
        {
            steps
            {
                echo "========Docker Building========"
            }
            post
            {
                success
                {
                    echo "========Docker Compose successfully========"
                }
                failure
                {
                    echo "========Docker Compose failed========"
                }
            }
        }
    }
}