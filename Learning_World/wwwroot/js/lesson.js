function loadContent(url, viewName, content, pushState = true) {
    if (pushState) {
        // Push state to history only when navigating, not during popstate
        history.pushState({ url, viewName }, '', url);
    }

    fetch(viewName)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text();
        })
        .then(html => {
            content.innerHTML = html;
        })
        .catch(error => console.error('Error loading view:', error));
}
const contentDiv1 = document.getElementById('main');


document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById('sidebar');
    const toggleSidebarBtn = document.getElementById('toggleSidebarBtn');
    const menuText = toggleSidebarBtn.querySelector('.menu-text');
    const lessonItems = document.querySelectorAll('.lesson-item');
    const partTitles = document.querySelectorAll('.part-title');


    // just push the url in the browser and excute the viewname


    // Toggle sidebar collapse
    toggleSidebarBtn.addEventListener('click', function () {
        sidebar.classList.toggle('collapsed');
        menuText.textContent = sidebar.classList.contains('collapsed') ? 'Show menu' : 'Hide menu';
    });

    // Handle lesson item clicks
    lessonItems.forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            selectLesson(this);
        });
    });

    // Handle part title clicks to expand/collapse
    partTitles.forEach(title => {
        title.addEventListener('click', function () {
            const icon = this.querySelector('i');
            icon.classList.toggle('bi-chevron-right');
            icon.classList.toggle('bi-chevron-down');
        });
    });

    // Set active lesson
    function setActive(element) {
        lessonItems.forEach(item => item.classList.remove('active-section'));
        if (element) {
            element.classList.add('active-section');
        }
    }

    // Select and load a lesson when click
    function selectLesson(element) {
        const lessonId = element.dataset.lessonId;
        const lessonType = element.dataset.lessonType;
        const moduleId = element.dataset.moduleId;

        setActive(element);
        loadContent(
            `/Learn/lesson/${moduleId}/${lessonType}/${lessonId}`,
            `/Learn/LessonDisplayPartialView/${moduleId}/${lessonType}/${lessonId}`,
            contentDiv1
        );
    }



    // Load the initial content when the page loads (not clicked on button)
    function loadInitialContent() {
        const urlSegments = window.location.pathname.split('/');
        const moduleId = urlSegments[3];
        const lessonType = urlSegments[4];
        const lessonId = urlSegments[5];

        if (moduleId && lessonType && lessonId) {
            const selectedLesson = document.querySelector(`.lesson-item[data-lesson-id="${lessonId}"][data-lesson-type="${lessonType}"]`);

            if (selectedLesson) {
                const partContainer = selectedLesson.closest('.part-container');
                const collapseElement = partContainer.querySelector('.collapse');

                const bootstrapCollapse = new bootstrap.Collapse(collapseElement, { toggle: false });
                bootstrapCollapse.show();

                setActive(selectedLesson);
                loadContent(
                    `/Learn/lesson/${moduleId}/${lessonType}/${lessonId}`,
                    `/Learn/LessonDisplayPartialView/${moduleId}/${lessonType}/${lessonId}`,
                    contentDiv1,
                    false  // Don't push state on initial load
                );
            }
        } else {
            //const firstLesson = document.querySelector('.lesson-item');
            //if (firstLesson) {
            //    selectLesson(firstLesson);
            //}
        }
    }

    // Handle browser back/forward button navigation
    window.addEventListener('popstate', function (event) {
        const state = event.state;

        if (state) {
            // Restore the state without pushing a new one
            loadContent(state.url, state.viewName, contentDiv1, state.scriptPath, false);

            const urlSegments = state.url.split('/');
            const moduleId = urlSegments[3];
            const lessonType = urlSegments[4];
            const lessonId = urlSegments[5];

            const selectedLesson = document.querySelector(`.lesson-item[data-lesson-id="${lessonId}"][data-lesson-type="${lessonType}"]`);

            if (selectedLesson) {
                const partContainer = selectedLesson.closest('.part-container');
                const collapseElement = partContainer.querySelector('.collapse');

                const bootstrapCollapse = new bootstrap.Collapse(collapseElement, { toggle: false });
                bootstrapCollapse.show();

                setActive(selectedLesson);
            }
        } else {
            loadInitialContent();
        }
    });

    // Load the appropriate content on initial load
    loadInitialContent();
});




function checkBtn( lessonId, moduleId) {
    const spanText = document.getElementById("completed-span")
    const lessonIcon = document.getElementById(`i-${lessonId}`)
    const btn = document.getElementById('mark-btn');
    fetch(`/Learn/CompleteLesson/${moduleId}/${lessonId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(response => {
            if (response.ok) {
                spanText.innerText = "Completed";
                btn.innerText = "Done";
                lessonIcon.className = 'bi bi-check lesson-icon'
            } else {
                console.error('Failed to mark the lesson as completed.');
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
}
function submitQuizForm() {
    const quizForm = document.getElementById('quizForm');
    console.log(quizForm);

    if (quizForm.checkValidity()) {
        const formData = new FormData(quizForm);

        fetch(quizForm.action, {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                displayResults(data); 
            })
            .catch(error => {
                console.error('Fetch error:', error);
                alert('An error occurred while submitting the quiz. Please try again.');
            });
    } else {
        alert("Please answer all the questions before submitting.");
    }
}

function displayResults(response ) {
    var resultMessage ="";
    if (response.passed) {
        resultMessage = "Congratulations! You passed the quiz.";
        const lessonIcon = document.getElementById(`i-${response.lessonId}`)
        lessonIcon.className = 'bi bi-check lesson-icon'
    }
    else {
        resultMessage = "You did not pass the quiz. Please review your answers.";
    }
    
    document.getElementById('resultMessage').textContent = `${resultMessage} You scored ${response.percentage.toFixed(2)}%.`;

    response.results.forEach(result => {
        const questionElement = document.querySelector(`input[name="answer_${result.questionId}"]`).closest('.mb-3');
        questionElement.querySelectorAll('.form-check-label').forEach(label => {
            const input = label.previousElementSibling; 
            label.classList.remove('text-success', 'text-danger', 'font-weight-bold');
            if (input.value == result.correctAnswerId && input.value == result.selectedAnswerId) {
                label.classList.add('text-success', 'font-weight-bold');
            }
            else if (input.value == result.selectedAnswerId && !result.isCorrect) {
                label.classList.add('text-danger');
            }
        });
    });

    document.getElementById('submitButton').innerText = 'Submit Again'; 

    document.getElementById('quizResults').style.display = 'block';
}


//function nextLesson(moduleId, lessonId, courseId) {
//    fetch(`/Learn/Next/${moduleId}/${lessonId}/${courseId}`, {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/json',
//        }
//    })
//        .then(response => {
//            if (!response.ok) {
//                throw new Error('Network response was not ok');
//            }
//            return response.json(); // Parse the response as JSON
//        })
//        .then(data => {
//            if (data !== null) {
//                // Extract lessonId, lessonType, and HTML content from the response
//                const newLessonId = data.lessonId;
//                const lessonType = data.lessonType;

//                // Update the browser's URL using history.pushState
//                const newUrl = `/Learn/lesson/${moduleId}/${lessonType}/${newLessonId}`;
//                const viewName = `/Learn/LessonDisplayPartialView/${moduleId}/${lessonType}/${newLessonId}`;
//                history.pushState({ url: newUrl, viewName: viewName }, '', newUrl);

//                // Directly load the content based on the new URL
//                loadContent(newUrl, viewName, contentDiv1, false);
//            }
//        })
//        .catch(error => {
//            console.error('Error:', error);
//        });
//}


