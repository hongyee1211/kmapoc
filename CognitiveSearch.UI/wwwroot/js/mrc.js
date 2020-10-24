function QueryMRC(question) {
    $.post('/api/mrc/ask',
        {
            title: currentSelectedPDF,
            content: transcriptFullContent,
            query: question,
        },
        function (data) {
            console.log(data);
            let resultHtml = `<p style="color:white">Answer: ${data.answer}</p>
                <p style="color:white">From: ${data.paragraph}</p>`
            $("#mrc-answer-body").html(resultHtml);
        }
    )
}